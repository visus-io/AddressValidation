---
title: Batch Mappers | Custom Integration
uid: custom-batch-mappers
---

## Batch Request Mapper

The batch request mapper translates an entire list of [request models](xref:custom-models) into a single provider DTO representing the whole batch, not a list of individual DTOs. Implement [`IBatchApiRequestMapper<TRequest, TApiRequest>`](xref:Visus.AddressValidation.Mappers.IBatchApiRequestMapper`2) with a single `Map` method.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchAddressValidationRequestMapper : IBatchApiRequestMapper<MyAddressValidationRequest, ApiRequest>
{
    public ApiRequest Map(IReadOnlyList<MyAddressValidationRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);

        return new ApiRequest
        {
            Addresses = [.. requests.Select(MyAddressPayloadMapper.Map),],
        };
    }
}
```

> [!NOTE]
> If mapping a single request to its payload shape is shared between the singular and batch request mappers, factor it into an internal static helper (`MyAddressPayloadMapper.Map` above) and call it from both. This is how the FedEx integration avoids duplicating per-address mapping logic.

> [!NOTE]
> Some providers accept only one value for fields that would otherwise be per-request, such as a transaction or correlation identifier. When that happens, take the value from the first request and document the behavior on the affected request property, as the FedEx integration does on `CustomerTransactionId`:
> ```csharp
> /// <remarks>
> ///     When submitted via <see cref="IBatchAddressValidationService{TRequest}" />, the provider accepts only
> ///     one transaction identifier for the entire batch call, so only the value set on the first request is
> ///     transmitted. Use a per-item field to correlate individual items within a batch.
> /// </remarks>
> public string? TransactionId { get; set; }
> ```

## Batch Response Mapper

The batch response mapper converts one item out of the provider's batch response into a unified [`IAddressValidationResponse`](xref:Visus.AddressValidation.Models.IAddressValidationResponse), selected by position. Implement [`IBatchApiResponseMapper<TApiResponse>`](xref:Visus.AddressValidation.Mappers.IBatchApiResponseMapper`1) with a single `Map` method.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchAddressValidationResponseMapper : IBatchApiResponseMapper<ApiResponse>
{
    public IAddressValidationResponse Map(ApiResponse response, int index, IValidationResult? validationResult = null)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        if ( response.Results is null || index >= response.Results.Length )
        {
            return new EmptyAddressValidationResponse(validationResult);
        }

        ApiResponse.ResultPayload result = response.Results[index];

        return new AddressValidationResponse(response, validationResult)
        {
            AddressLines = result.AddressLines is not null
                               ? result.AddressLines.ToFrozenSet(StringComparer.OrdinalIgnoreCase)
                               : FrozenSet<string>.Empty,
            CityOrTown = result.City,
            StateOrProvince = result.State,
            PostalCode = result.PostalCode,
        };
    }
}
```

> [!NOTE]
> `index` is the position of the item within the provider's response, not the original caller-facing index of the request. The [batch validation service](xref:Visus.AddressValidation.Services.AbstractBatchAddressValidationService`2) only calls `Map` for items that were actually sent to the provider (locally-invalid requests are already resolved to an `EmptyAddressValidationResponse` before the API call), so `index` always lines up with the provider response array.

> [!NOTE]
> When response fields are shared between the singular and batch response DTOs, factor the per-item mapping logic (including `GetCustomResponseData()` handling, see [Data Models](xref:custom-models)) into an internal static helper and call it from both the singular and batch response mappers.

> [!NOTE]
> It is not necessary for either batch mapper to be `internal`, but it is **strongly** recommended if redistributing as a library.
