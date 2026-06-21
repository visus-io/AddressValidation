---
title: Request Adapter | Custom Integration
uid: custom-request-adapter
---

## Request Adapter

The request adapter bridges the public request to the [validation client](xref:custom-validation-client). It maps the incoming [request model](xref:custom-request-model) to the provider DTO using the [request mapper](xref:custom-request-mapper), then forwards it to the typed HTTP client. Implement [`IApiRequestAdapter<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Adapters.IApiRequestAdapter`2).

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
