---
title: Pitney Bowes
---

# Pitney Bowes Integration

AddressValidation offers a complete integration to the [Pitney Bowes Address Validation API](https://docs.shippingapi.pitneybowes.com/address-validation.html) to provide validation for the United States and Puerto Rico.

## Credentials

Before utilizing the integration, you will need a [developer account](https://docs.shippingapi.pitneybowes.com/getting-started.html#sign-up-or-log-in) with Pitney Bowes. After you have signed in to the account, [follow these instructions](https://docs.shippingapi.pitneybowes.com/getting-started.html#get-your-api-key-and-secret) to obtain your API key and secret. 

> [!NOTE]
> Production access is not granted by default. You will need [contact](https://docs.shippingapi.pitneybowes.com/getting-started.html#upgrade-to-production) Pitney Bowes to enable Production.

> [!TIP]
> Service providers should read up on [Merchant Accounts](https://docs.shippingapi.pitneybowes.com/merchant-accounts.html) should you wish to provide other services to clients. You may continue to use your own `Developer ID` for this integration as the Address Validation API does not require a `Shipper ID`.

## Installation

The easiest way to install the integration into a project is through NuGet:

# [.NET CLI](#tab/tab-ave-pitney-bowes-cli)
```Shell
dotnet package add Visus.AddressValidation.Integration.PitneyBowes
```
# [Package Manager](#tab/tab-ave-pitney-bowes-pm)
```PowerShell
Install-Package Visus.AddressValidation.Integration.PitneyBowes
```
---

At application startup, you will need to register the integration with the [Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) container:

```csharp
builder.Services.AddPitneyBowesAddressValidation();
```

[!INCLUDE [distributed-cache-required](../includes/distributed-cache-required.md)]

## Configuration

Configuration of the integration is relatively straight forward and all parameters are read through an `IConfiguration` instance.

| Key                            | Notes                                                                                               |
|--------------------------------|-----------------------------------------------------------------------------------------------------|
| `VS_AVE_PB_DEVELOPER_ID`       | [Developer ID](https://docs.shippingapi.pitneybowes.com/getting-started.html#get-your-developer-id) |
| `VS_AVE_PB_CLIENT_ENVIRONMENT` | Accepted values are `PRODUCTION` or `DEVELOPMENT`                                                   |
| `VS_AVE_PB_API_KEY`            |                                                                                                     |
| `VS_AVE_PB_API_SECRET`         |                                                                                                     |

> [!NOTE]
> `VS_AVE_PB_ACCOUNT_NUMBER` is only used for the purposes of constructing a cache key for the authentication cache.

> [!IMPORTANT]
> `VS_AVE_PB_API_KEY` and `VS_AVE_PB_API_SECRET` should be stored encrypted at rest. See the [Security](../index.md#security) for additional details.

## Standard Example

The following example demonstrates a standard address validation request. 
For situations where the validation has failed or the address being provided is incomplete, consider making an
[address suggestion request](#suggestion-example) instead.

```csharp
public class ValidateController(IAddressValidationService<PitneyBowesAddressValidationRequest> validationService)
{
    private readonly IAddressValidationService<PitneyBowesAddressValidationRequest> _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    
    [HttpGet]
    public async ValueTask<IActionResult> Get()
    {
        new PitneyBowesAddressValidationRequest
        {
            AddressLines =
            {
                "1600 Amphitheatre Pkwy"
            },
            CityOrTown = "Mountain View",
            StateOrProvince = "CA",
            PostalCode = "94043",
            Country = CountryCode.US            
        };
        
        IAddressValidationResponse? response = await ValidationService.ValidateAsync(request);
        
        return response is null
            ? new NoContentResult()
            : new OkObjectResult(response);
    }
}
```
# [Request](#tab/tab-ave-pitney-bowes-json-request)
```JSON
{
  "addressLines": [
    "1600 Amphitheatre Pkwy"
  ],
  "cityTown": "Mountain View",
  "stateProvince": "CA",
  "postalCode": "94043",
  "countryCode": "US"
}
```
# [Response](#tab/tab-ave-pitney-bowes-json-response)
```JSON
{
  "addressLines": [
    "1600 AMPHITHEATRE PKWY"
  ],
  "cityOrTown": "MOUNTAIN VIEW",
  "country": "US",
  "errors": [],
  "isResidential": false,
  "postalCode": "94043-1351",
  "stateOrProvince": "CA",
  "suggestions": [],
  "warnings": []
}
```
---

[!INCLUDE [is-residential-note](../includes/is-residential-note.md)]

## Suggestion Example

The following example demonstrates an address suggestion request. Scenarios in which such requests are made include:

- [Standard request](#standard-example) returned a validation failure in the [Errors](xref:Visus.AddressValidation.Model.IAddressValidationResponse#Visus_AddressValidation_Model_IAddressValidationResponse_Errors) collection.
- Provided address is incomplete or ambiguous.

In order to trigger an address suggestion request, set the `IncludeSuggestions` property to `true`.

```csharp
public class ValidateController(IAddressValidationService<PitneyBowesAddressValidationRequest> validationService)
{
    private readonly IAddressValidationService<PitneyBowesAddressValidationRequest> _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    
    [HttpGet]
    public async ValueTask<IActionResult> Get()
    {
        new PitneyBowesAddressValidationRequest
        {
            AddressLines =
            {
                "3 1st Ave NW"
            },
            CityOrTown = "Litz",
            StateOrProvince = "FL",
            PostalCode = "33549",
            Country = CountryCode.US,
            IncludeSuggestions = true # triggers an address suggestion request           
        };
        
        IAddressValidationResponse? response = await ValidationService.ValidateAsync(request);
        
        return response is null
            ? new NoContentResult()
            : new OkObjectResult(response);
    }
}
```

# [Suggestion Request](#tab/tab-ave-pitney-bowes-json-suggest-request)
```JSON
{
  "address": {
    "addressLines": [
      "3 1st Ave NW"
    ],
    "cityTown": "Litz",
    "stateProvince": "FL",
    "postalCode": "33549",
    "countryCode": "US"
  }
}
```
# [Suggestion Response](#tab/tab-ave-pitney-bowes-json-suggest-response)
```JSON
{
  "addressLines": [
    "3 1ST AVE NW"
  ],
  "cityOrTown": "LITZ",
  "country": "US",
  "errors": [],
  "isResidential": null,
  "postalCode": "33549",
  "stateOrProvince": "FL",
  "suggestions": [
    {
      "addressLines": [
        "3 1ST AVE NE"
      ],
      "cityOrTown": "LITZ",
      "country": "US",
      "errors": [],
      "isResidential": null,
      "postalCode": "33549",
      "stateOrProvince": "FL",
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