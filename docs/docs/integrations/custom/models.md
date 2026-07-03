---
title: Data Models | Custom Integration
uid: custom-models
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

## Provider API Contracts

Provider API contracts are `internal sealed` DTO classes that mirror the JSON payload sent to and received from the provider API. By convention these are named `ApiRequest` and `ApiResponse` and placed in a `Contracts` namespace.

```csharp
internal sealed class ApiRequest
{
    public AddressPayload? Address { get; set; }

    internal sealed class AddressPayload
    {
        public string[]? Lines { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryCode { get; set; }
    }
}

internal sealed partial class ApiResponse
{
    public ResultPayload? Result { get; set; }

    [CustomResponseDataProperty]
    public string? TransactionId { get; set; }

    public ErrorPayload[]? Errors { get; set; }

    internal sealed partial class ResultPayload
    {
        public string[]? AddressLines { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        [CustomResponseDataProperty]
        public bool IsResidential { get; set; }

        [CustomResponseDataProperty]
        [JsonPropertyName("DPV")]
        public string? DeliveryPointValidation { get; set; }
    }

    internal sealed class ErrorPayload
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }
}
```

When a provider returns errors on non-2xx responses using a different JSON shape than the success body, define a peer `ApiErrorResponse` class alongside `ApiResponse`:

```csharp
internal sealed class ApiErrorResponse
{
    public ApiResponse.ErrorPayload[]? Errors { get; set; }
}
```

The [Validation Client](xref:custom-validation-client) deserializes this type separately on failure and copies its `Errors` into an `ApiResponse` for the pipeline to process.

> [!NOTE]
> Only add `[JsonPropertyName]` when the provider's JSON field name differs from what the serializer produces for the C# property name. Most properties do not need it. For example, if a field is named `"DPV"` in JSON but the C# property is `DeliveryPointValidation`, the attribute is required; when names already match, omit it.

> [!NOTE]
> These types are the input and output for the JSON source-generated serializer contexts used by the [Validation Client](xref:custom-validation-client). Each DTO type used with `JsonContent.Create(...)` or `ReadFromJsonAsync(...)` must be listed in a `[JsonSerializable(...)]` attribute on the serializer context. This includes `ApiErrorResponse` — it is deserialized independently on non-2xx responses and must have its own entry alongside `ApiResponse`.

## Surfacing Custom Response Data

[`[CustomResponseDataProperty]`](xref:Visus.AddressValidation.CustomResponseDataPropertyAttribute) marks a response contract property for inclusion in [`IAddressValidationResponse.CustomResponseData`](xref:Visus.AddressValidation.Models.IAddressValidationResponse.CustomResponseData) (`IReadOnlyDictionary<string, object?>`). This lets consumers inspect provider-specific fields — residency classification, delivery point data, transaction identifiers, and so on — without casting to any provider-specific type.

The attribute has two forms:

- **`[CustomResponseDataProperty]`** — the C# property name is converted to camelCase and used as the dictionary key (`TransactionId` → `"transactionId"`).
- **`[CustomResponseDataProperty("customKey")]`** — the supplied string is used as the dictionary key as-is.

When a property needs both a different JSON name and a place in `CustomResponseData`, apply both attributes:

```csharp
[CustomResponseDataProperty]          // key: "deliveryPointValidation"
[JsonPropertyName("DPV")]             // deserializes from JSON field "DPV"
public string? DeliveryPointValidation { get; set; }
```

The `Visus.AddressValidation.SourceGeneration` package generates a `GetCustomResponseData()` method at compile time on any type that has at least one decorated property. No runtime reflection is involved. See the [Response Mapper](xref:custom-mappers) page for how to call it and assign `CustomResponseData` on the unified response.

Any type that has at least one `[CustomResponseDataProperty]`-decorated property must be declared `partial`. When the decorated type is a nested class, every enclosing type in the chain must also be `partial` so the generator can emit the `GetCustomResponseData()` method inside the correct nested class hierarchy. Types with no decorated properties — such as `ErrorPayload` above — do not need to be `partial`.
