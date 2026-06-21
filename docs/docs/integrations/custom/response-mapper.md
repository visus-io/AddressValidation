---
title: Response Mapper | Custom Integration
uid: custom-response-mapper
---

## Response Mapper

The response mapper converts the provider's raw [API response](xref:custom-contracts) into the unified [`IAddressValidationResponse`](xref:Visus.AddressValidation.Models.IAddressValidationResponse) returned to callers. Implement [`IApiResponseMapper<TApiResponse>`](xref:Visus.AddressValidation.Mappers.IApiResponseMapper`1) with a single `Map` method.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationResponseMapper : IApiResponseMapper<ApiResponse>
{
    public IAddressValidationResponse Map(ApiResponse response, IValidationResult? validationResult = null)
    {
        if ( response.Result is null )
        {
            return new EmptyAddressValidationResponse(validationResult);
        }

        return new AddressValidationResponse(response, validationResult)
        {
            AddressLines = response.Result.AddressLines is not null
                               ? response.Result.AddressLines.ToFrozenSet(StringComparer.OrdinalIgnoreCase)
                               : FrozenSet<string>.Empty,
            CityOrTown = response.Result.City,
            StateOrProvince = response.Result.State,
            PostalCode = response.Result.PostalCode,
        };
    }
}
```

> [!NOTE]
> The concrete return type — `AddressValidationResponse` in the example above — should extend [`AbstractAddressValidationResponse<TResponse>`](xref:Visus.AddressValidation.Models.AbstractAddressValidationResponse`1) where `TResponse` is the provider DTO (`ApiResponse`). The base class automatically populates the `Errors` and `Warnings` collections from the `validationResult`.
>
> ```csharp
> internal sealed class AddressValidationResponse : AbstractAddressValidationResponse<ApiResponse>
> {
>     public AddressValidationResponse(ApiResponse response, IValidationResult? validationResult = null)
>         : base(response, validationResult)
>     {
>     }
> }
> ```

> [!NOTE]
> It is not necessary for the response mapper to be `internal`, but it is **strongly** recommended if redistributing as a library.
