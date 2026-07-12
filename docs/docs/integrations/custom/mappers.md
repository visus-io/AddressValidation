---
title: Mappers | Custom Integration
uid: custom-mappers
---

## Request Mapper

The request mapper translates the public-facing [request model](xref:custom-models) into the provider-specific [API contract](xref:custom-models). Implement [`IApiRequestMapper<TRequest, TApiRequest>`](xref:Visus.AddressValidation.Mappers.IApiRequestMapper`2) with a single `Map` method.

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

## Response Mapper

The response mapper converts the provider's raw [API response](xref:custom-models) into the unified [`IAddressValidationResponse`](xref:Visus.AddressValidation.Models.IAddressValidationResponse) returned to callers. Implement [`IApiResponseMapper<TApiResponse>`](xref:Visus.AddressValidation.Mappers.IApiResponseMapper`1) with a single `Map` method.

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
            CustomResponseData = response.GetCustomResponseData(),
        };
    }
}
```

> [!NOTE]
> `GetCustomResponseData()` is generated at compile time by the source generator bundled in `VisusIO.AddressValidation` for every contract type that has at least one [`[CustomResponseDataProperty]`](xref:Visus.AddressValidation.CustomResponseDataPropertyAttribute)-decorated property. It returns `IReadOnlyDictionary<string, object?>`. See [Data Models](xref:custom-models) for how to annotate contract properties.
>
> When custom data is spread across multiple nested contract types, merge the dictionaries before assigning:
>
> ```csharp
> Dictionary<string, object?> customResponseData = new(StringComparer.OrdinalIgnoreCase);
> customResponseData.Merge(response.GetCustomResponseData());
> customResponseData.Merge(response.Result.GetCustomResponseData());
>
> return new AddressValidationResponse(response, validationResult)
> {
>     // ...
>     CustomResponseData = customResponseData.AsReadOnly(),
> };
> ```

> [!NOTE]
> The concrete return type (`AddressValidationResponse` in the example above) should extend [`AbstractAddressValidationResponse<TResponse>`](xref:Visus.AddressValidation.Models.AbstractAddressValidationResponse`1) where `TResponse` is the provider DTO (`ApiResponse`). The base class automatically populates the `Errors` and `Warnings` collections from the `validationResult`.
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
