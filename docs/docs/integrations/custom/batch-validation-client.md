---
title: Batch Validation Client | Custom Integration
uid: custom-batch-validation-client
---

## Batch Request Adapter

The batch request adapter bridges a list of public requests to the [validation client](xref:custom-validation-client). It maps the incoming requests to the provider's batch DTO using the [batch request mapper](xref:custom-batch-mappers), then forwards it to the same typed `HttpClient` used by the singular pipeline. Implement [`IBatchApiRequestAdapter<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Adapters.IBatchApiRequestAdapter`2).

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchApiRequestAdapter : IBatchApiRequestAdapter<MyAddressValidationRequest, ApiResponse>
{
    private readonly MyAddressValidationClient _client;

    private readonly IBatchApiRequestMapper<MyAddressValidationRequest, ApiRequest> _requestMapper;

    public BatchApiRequestAdapter(MyAddressValidationClient client,
                                  IBatchApiRequestMapper<MyAddressValidationRequest, ApiRequest> requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
    }

    public Task<ApiResponse?> ExecuteAsync(IReadOnlyList<MyAddressValidationRequest> requests, CancellationToken cancellationToken)
    {
        ApiRequest apiRequest = _requestMapper.Map(requests);
        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
```

> [!NOTE]
> No new authentication or HTTP infrastructure is needed for batch support. The adapter reuses the same [validation client](xref:custom-validation-client) and, transitively, the same [`BearerTokenDelegatingHandler<TClient>`](xref:Visus.AddressValidation.Http.BearerTokenDelegatingHandler`1) and resilience pipeline configured for the singular pipeline. If the provider's batch endpoint is a different URL or accepts a different request shape than the single-address endpoint, add a second method to the existing validation client rather than creating a separate typed `HttpClient`.

```csharp
public Task<ApiResponse?> ValidateAddressesAsync(ApiRequest request, CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(request);
    return ValidateAddressesInternalAsync(request, cancellationToken);
}

private async Task<ApiResponse?> ValidateAddressesInternalAsync(ApiRequest request, CancellationToken cancellationToken)
{
    Uri requestUri = new(_options.Value.EndpointUri, "/v1/address/validate-batch");

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
    return errorResponse is not null ? new ApiResponse { ErrorResponse = errorResponse } : null;
}
```

> [!NOTE]
> It is not necessary for the batch request adapter to be `internal`, but it is **strongly** recommended if redistributing as a library.
