---
title: Request Mapper | Custom Integration
uid: custom-request-mapper
---

## Request Mapper

The request mapper translates the public-facing [request model](xref:custom-request-model) into the provider-specific [API contract](xref:custom-contracts). Implement [`IApiRequestMapper<TRequest, TApiRequest>`](xref:Visus.AddressValidation.Mappers.IApiRequestMapper`2) with a single `Map` method.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationRequestMapper : IApiRequestMapper<MyAddressValidationRequest, ApiRequest>
{
    public ApiRequest Map(MyAddressValidationRequest request)
    {
        return new ApiRequest
        {
            Address = new ApiRequest.AddressPayload
            {
                Lines = [.. request.AddressLines],
                City = request.CityOrTown,
                State = request.StateOrProvince,
                PostalCode = request.PostalCode,
                CountryCode = request.Country?.Value,
            },
        };
    }
}
```

> [!NOTE]
> It is not necessary for the request mapper to be `internal`, but it is **strongly** recommended if redistributing as a library.
