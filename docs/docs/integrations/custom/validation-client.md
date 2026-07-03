---
title: Validation Client | Custom Integration
uid: custom-validation-client
---

## Validation Client

The validation client is a [typed `HttpClient`](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#how-to-use-typed-clients-with-ihttpclientfactory) whose sole responsibility is calling the provider's address validation endpoint and returning the raw response DTO.

Below is an example of a validation client that sends a `POST` request to the provider API and deserializes the result:

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class MyAddressValidationClient
{
    private readonly HttpClient _httpClient;

    private readonly IOptions<MyServiceOptions> _options;

    public MyAddressValidationClient(HttpClient httpClient, IOptions<MyServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<ApiResponse?> ValidateAddressAsync(ApiRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateAddressInternalAsync(request, cancellationToken);
    }

    private async Task<ApiResponse?> ValidateAddressInternalAsync(ApiRequest request, CancellationToken cancellationToken)
    {
        Uri requestUri = new(_options.Value.EndpointUri, "/v1/address/validate");

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, requestUri);

        httpRequest.Content = JsonContent.Create(request, MyApiRequestJsonSerializerContext.Default.ApiRequest);

        using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        if ( response.IsSuccessStatusCode )
        {
            return await response.Content.ReadFromJsonAsync(MyApiResponseJsonSerializerContext.Default.ApiResponse, cancellationToken)
                                         .ConfigureAwait(false);
        }

        ApiErrorResponse? errorResponse = await response.Content.ReadFromJsonAsync(MyApiResponseJsonSerializerContext.Default.ApiErrorResponse, cancellationToken)
                                                                 .ConfigureAwait(false);
        if ( errorResponse is not null )
        {
            return new ApiResponse { Errors = errorResponse.Errors };
        }

        return null;
    }
}
```

> [!NOTE]
> The [`JsonContent.Create(...)`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.jsoncontent.create) and [`ReadFromJsonAsync(...)`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.httpcontentjsonextensions.readfromjsonasync) calls reference [source-generated](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation) JSON serializer contexts. Create one `internal sealed partial class` per DTO direction, annotated with `[JsonSerializable(...)]`:
> ```csharp
> [JsonSerializable(typeof(ApiRequest))]
> [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
>     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
> internal sealed partial class MyApiRequestJsonSerializerContext : JsonSerializerContext { }
>
> [JsonSerializable(typeof(ApiResponse))]
> [JsonSerializable(typeof(ApiErrorResponse))]
> [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
> internal sealed partial class MyApiResponseJsonSerializerContext : JsonSerializerContext { }
> ```
> Source-generated contexts are required for correct behavior in [trimmed](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming) and [native AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/) deployments.

> [!NOTE]
> Unlike the [Authentication Client](xref:custom-authentication), the validation client does **not** implement an interface. It is referenced directly by its concrete type from the request adapter.

> [!IMPORTANT]
> The `Authorization` header must be redacted from HTTP logs. This is configured during [service registration](xref:custom-registering-services) via `RedactLoggedHeaders(["Authorization"])` on the `IHttpClientBuilder` — there is nothing to configure in the client itself. Without this configuration, bearer tokens will appear in structured logs.

> [!NOTE]
> It is not necessary for the validation client to be `internal`, but it is **strongly** recommended if redistributing as a library.

## Request Adapter

The request adapter bridges the public request to the [validation client](#validation-client). It maps the incoming [request model](xref:custom-models) to the provider DTO using the [request mapper](xref:custom-mappers), then forwards it to the typed HTTP client. Implement [`IApiRequestAdapter<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Adapters.IApiRequestAdapter`2).

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class ApiRequestAdapter : IApiRequestAdapter<MyAddressValidationRequest, ApiResponse>
{
    private readonly MyAddressValidationClient _client;

    private readonly IApiRequestMapper<MyAddressValidationRequest, ApiRequest> _requestMapper;

    public ApiRequestAdapter(MyAddressValidationClient client,
                             IApiRequestMapper<MyAddressValidationRequest, ApiRequest> requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
    }

    public Task<ApiResponse?> ExecuteAsync(MyAddressValidationRequest request, CancellationToken cancellationToken)
    {
        ApiRequest apiRequest = _requestMapper.Map(request);
        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
```

> [!NOTE]
> It is not necessary for the request adapter to be `internal`, but it is **strongly** recommended if redistributing as a library.
