---
title: Pitney Bowes
---

# Pitney Bowes Integration

AddressValidation offers a complete integration to the [Pitney Bowes Address Validation API](https://docs.shippingapi.pitneybowes.com/address-validation.html) to provide validation for the United States and Puerto Rico.

[!INCLUDE [batch-validation-not-supported-note](../includes/batch-validation-not-supported-note.md)]

## Credentials

Get a [developer account](https://docs.shippingapi.pitneybowes.com/getting-started.html#sign-up-or-log-in) with Pitney Bowes before you use this integration. After you sign in, [follow these instructions](https://docs.shippingapi.pitneybowes.com/getting-started.html#get-your-api-key-and-secret) to get your API key and secret.

> [!NOTE]
> Production access is not granted by default. [Contact](https://docs.shippingapi.pitneybowes.com/getting-started.html#upgrade-to-production) Pitney Bowes to enable Production.

> [!TIP]
> If you provide other services to clients, read about [Merchant Accounts](https://docs.shippingapi.pitneybowes.com/merchant-accounts.html). You can still use your own `Developer ID` for this integration — the Address Validation API does not require a `Shipper ID`.

## Installation

The easiest way to install the integration into a project is through NuGet:

# [.NET CLI](#tab/tab-ave-pitney-bowes-cli)
```Shell
dotnet package add VisusIO.AddressValidation.Integration.PitneyBowes
```
# [Package Manager](#tab/tab-ave-pitney-bowes-pm)
```PowerShell
Install-Package VisusIO.AddressValidation.Integration.PitneyBowes
```
---

Register the integration with the [Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) container at application startup:

```csharp
builder.Services.AddPitneyBowesAddressValidation();
```

[!INCLUDE [hybrid-cache-required](../includes/hybrid-cache-required.md)]

## Configuration

Configuration is bound from the `AddressValidationSettings:PitneyBowes` section.

```json
{
  "AddressValidationSettings": {
    "PitneyBowes": {
      "DeveloperId": "<your developer id>",
      "ApiKey": "<your api key>",
      "ApiSecret": "<your api secret>",
      "ClientEnvironment": "PRODUCTION"
    }
  }
}
```

| Property | Required | Description |
|---|---|---|
| `DeveloperId` | Yes | [Developer ID](https://docs.shippingapi.pitneybowes.com/getting-started.html#get-your-developer-id) associated with your registered application |
| `ApiKey` | Yes | API key issued by Pitney Bowes for your registered application |
| `ApiSecret` | Yes | API secret issued by Pitney Bowes for your registered application |
| `ClientEnvironment` | No | Accepted values: `PRODUCTION`, `DEVELOPMENT`, `SANDBOX`. Defaults to `DEVELOPMENT` |
| `EndpointUriOverride` | SANDBOX only | Custom endpoint URI; required when `ClientEnvironment` is `SANDBOX` |

> [!NOTE]
> `DeveloperId` is only used to construct the cache key for the authentication cache.

> [!IMPORTANT]
> Store `ApiKey` and `ApiSecret` encrypted at rest. See [Security](../index.md#security) for more details.

## Standard Example

This example shows a standard address validation request. If validation fails, or the address is incomplete, make an
[address suggestion request](#suggestion-example) instead.

```csharp
public class ValidateController
{
    private readonly IAddressValidationService<PitneyBowesAddressValidationRequest> _validationService;

    public ValidateController(IAddressValidationService<PitneyBowesAddressValidationRequest> validationService)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PitneyBowesAddressValidationRequest request, CancellationToken cancellationToken = default)
    {
        IAddressValidationResponse? response = await _validationService.ValidateAsync(request, cancellationToken);
        
        return response is null
            ? new NotFoundResult()
            : response.Errors.Count > 0
                ? new UnprocessableEntityObjectResult(response)
                : new OkObjectResult(response);
    }
}
```
# [Request](#tab/tab-ave-pitney-bowes-json-request)
```JSON
{
  "addressLines": [
    "350 5th Ave"
  ],
  "cityTown": "New York",
  "stateProvince": "NY",
  "postalCode": "10118",
  "countryCode": "US"
}
```
# [Response](#tab/tab-ave-pitney-bowes-json-response)
```JSON
{
  "addressLines": [
    "350 5TH AVE"
  ],
  "cityOrTown": "NEW YORK",
  "country": "US",
  "errors": [],
  "isResidential": false,
  "postalCode": "10118-0001",
  "stateOrProvince": "NY",
  "suggestions": [],
  "warnings": []
}
```
---

[!INCLUDE [is-residential-note](../includes/is-residential-note.md)]

## Suggestion Example

This example shows an address suggestion request. Make this request in these scenarios:

- [Standard request](#standard-example) returned a validation failure in the [Errors](xref:Visus.AddressValidation.Models.IAddressValidationResponse#Visus_AddressValidation_Models_IAddressValidationResponse_Errors) collection.
- Provided address is incomplete or ambiguous.

To trigger an address suggestion request, set the `IncludeSuggestions` property to `true`.

# [Suggestion Request](#tab/tab-ave-pitney-bowes-json-suggest-request)
```JSON
{
  "addressLines": [
    "30 Rockefeller Plz"
  ],
  "cityTown": "New York",
  "stateProvince": "NY",
  "postalCode": "10112",
  "countryCode": "US"
}
```
# [Suggestion Response](#tab/tab-ave-pitney-bowes-json-suggest-response)
```JSON
{
  "addressLines": [
    "30 ROCKEFELLER PLZ"
  ],
  "cityOrTown": "NEW YORK",
  "country": "US",
  "errors": [],
  "isResidential": false,
  "postalCode": "10112-0015",
  "stateOrProvince": "NY",
  "suggestions": [
    {
      "addressLines": [
        "1270 6TH AVE"
      ],
      "cityOrTown": "NEW YORK",
      "country": "US",
      "errors": [],
      "isResidential": false,
      "postalCode": "10020-1406",
      "stateOrProvince": "NY",
      "suggestions": [],
      "warnings": []
    }
  ],
  "warnings": []
}
```
---

[!INCLUDE [suggestions-need-validation-note](../includes/suggestions-need-validation-note.md)]

[!INCLUDE [is-residential-note](../includes/is-residential-note.md)]

[!INCLUDE [internal-validation-note](../includes/internal-validation-note.md)]
