---
title: Validation Service | Custom Integration
uid: custom-validation-service
---

## Validation Service

The validation service wires all pipeline components together. Extend [`AbstractAddressValidationService<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Services.AbstractAddressValidationService`2) and forward the four constructor dependencies to the base class.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationService : AbstractAddressValidationService<MyAddressValidationRequest, ApiResponse>
{
    public AddressValidationService(IApiRequestAdapter<MyAddressValidationRequest, ApiResponse> requestAdapter,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IValidator<MyAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestAdapter, responseMapper, requestValidator, responseValidator)
    {
    }
}
```

> [!NOTE]
> No additional logic belongs here. The base class manages the full validation pipeline: pre-validating the request, calling the API via the [request adapter](xref:custom-request-adapter), validating the response, and mapping it to an [`IAddressValidationResponse`](xref:Visus.AddressValidation.Models.IAddressValidationResponse).

> [!NOTE]
> It is not necessary for the validation service to be `internal`, but it is **strongly** recommended if redistributing as a library.
