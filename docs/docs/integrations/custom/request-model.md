---
title: Request Model | Custom Integration
uid: custom-request-model
---

## Request Model

The request model is the public-facing type consumers pass to [`IAddressValidationService<TRequest>`](xref:Visus.AddressValidation.Services.IAddressValidationService`1). Extend [`AbstractAddressValidationRequest`](xref:Visus.AddressValidation.Models.AbstractAddressValidationRequest) to inherit all standard address fields (`AddressLines`, `CityOrTown`, `StateOrProvince`, `PostalCode`, `Country`) and add only provider-specific properties.

```csharp
public sealed class MyAddressValidationRequest : AbstractAddressValidationRequest
{
    public int? MaximumResults { get; set; }
}
```

> [!NOTE]
> If the provider does not require any additional fields beyond the standard address properties, the class body can be left empty. The subclass is still required so that the DI container can distinguish your integration's service from others.
