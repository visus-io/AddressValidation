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
    [JsonPropertyName("address")]
    public AddressPayload? Address { get; set; }

    internal sealed class AddressPayload
    {
        [JsonPropertyName("lines")]
        public string[]? Lines { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }
    }
}

internal sealed class ApiResponse
{
    [JsonPropertyName("result")]
    public ResultPayload? Result { get; set; }

    [JsonPropertyName("errors")]
    public ErrorPayload[]? Errors { get; set; }

    internal sealed class ResultPayload
    {
        [JsonPropertyName("addressLines")]
        public string[]? AddressLines { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }
    }

    internal sealed class ErrorPayload
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
```

> [!NOTE]
> These types are the input and output for the JSON source-generated serializer contexts used by the [Validation Client](xref:custom-validation-client). Each DTO type used with `JsonContent.Create(...)` or `ReadFromJsonAsync(...)` must be listed in a `[JsonSerializable(...)]` attribute on the serializer context.
