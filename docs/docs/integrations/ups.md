---
title: UPS
---

# UPS&reg; Integration

AddressValidation offers a complete integration to the [UPS&reg; Address Validation API](https://developer.ups.com/api/reference) to provide validation for the United States and Puerto Rico.

## Credentials

Before utilizing the integration, you will need a [developer account](https://developer.ups.com/) along with an active UPS account. After you have signed in to the account, [follow these instructions](https://developer.ups.com/get-started) to obtain your credentials.

## Installation

The easiest way to install the integration into a project is through NuGet:

# [.NET CLI](#tab/tab-ave-pitney-bowes-cli)
```Shell
dotnet package add Visus.AddressValidation.Integration.Ups
```
# [Package Manager](#tab/tab-ave-pitney-bowes-pm)
```PowerShell
Install-Package Visus.AddressValidation.Integration.Ups
```
---

At application startup, you will need to register the integration with the [Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) container:

```csharp
builder.Services.AddUpsAddressValidation();
```

[!INCLUDE [distributed-cache-required](../includes/distributed-cache-required.md)]

## Configuration

Configuration of the integration is relatively straight forward and all parameters are read through an `IConfiguration` instance.


| Key                             | Notes                                             |
|---------------------------------|---------------------------------------------------|
| `VS_AVE_UPS_ACCOUNT_NUMBER`     |                                                   |                                                                               
| `VS_AVE_UPS_CLIENT_ENVIRONMENT` | Accepted values are `PRODUCTION` or `DEVELOPMENT` |
| `VS_AVE_UPS_CLIENT_ID`          |                                                   |
| `VS_AVE_UPS_CLIENT_SECRET`      |                                                   |

> [!IMPORTANT]
> `VS_AVE_UPS_CLIENT_ID` and `VS_AVE_UPS_CLIENT_SECRET` should be stored encrypted at rest. See the [Security](../index.md#security) for additional details.

## Standard Example

The following example demonstrates a standard address validation request.

```csharp
public class ValidateController(IAddressValidationService<UpsAddressValidationRequest> validationService)
{
    private readonly IAddressValidationService<UpsAddressValidationRequest> _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    
    [HttpGet]
    public async ValueTask<IActionResult> Get()
    {
        // UPS Customer Center - Los Angeles, CA US
        new UpsAddressValidationRequest
        {
            AddressLines =
            {
                "1800 N Main St"
            },
            CityOrTown = "Los Angeles",
            StateOrProvince = "CA",
            PostalCode = "90031",
            Country = CountryCode.US            
        };
        
        IAddressValidationResponse? response = await ValidationService.ValidateAsync(request);
        
        return response is null
            ? new NoContentResult()
            : new OkObjectResult(response);
    }
}
```

> [!NOTE]
> When `VS_AVE_UPS_CLIENT_ENVIRONMENT` is set to `DEVELOPMENT` only addresses in New York (`NY`) and California (`CA`) are supported.

# [Request](#tab/tab-ave-pitney-bowes-json-request)
```JSON
{
  "XAVRequest": {
    "AddressKeyFormat": {
      "AddressLine": [
        "1800 N Main St"
      ],
      "PoliticalDivision2": "Los Angeles",
      "PoliticalDivision1": "CA",
      "PostcodePrimaryLow": "90031",
      "CountryCode": "US"
    }
  }
}
```
# [Response](#tab/tab-ave-pitney-bowes-json-response)
```JSON
{
  "addressLines": [
    "1800 N MAIN ST"
  ],
  "cityOrTown": "LOS ANGELES",
  "country": "US",
  "errors": [],
  "isResidential": true,
  "postalCode": "90031-3262",
  "stateOrProvince": "CA",
  "suggestions": [],
  "warnings": []
}
```
---

## Suggestion Example

In the event of an incomplete or ambiguous request, a potential match along with suggestions may be returned.

```csharp
public class ValidateController(IAddressValidationService<UpsAddressValidationRequest> validationService)
{
    private readonly IAddressValidationService<UpsAddressValidationRequest> _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    
    [HttpGet]
    public async ValueTask<IActionResult> Get()
    {
        // UPS Customer Center - Los Angeles, CA US (Malformed Address)
        new UpsAddressValidationRequest
        {
            AddressLines =
            {
                "1800 Main St"
            },
            CityOrTown = "Los Angeles",
            StateOrProvince = "CA",
            PostalCode = "90025",
            Country = CountryCode.US            
        };
        
        IAddressValidationResponse? response = await ValidationService.ValidateAsync(request);
        
        return response is null
            ? new NoContentResult()
            : new OkObjectResult(response);
    }
}
```

> [!NOTE]
> When `VS_AVE_UPS_CLIENT_ENVIRONMENT` is set to `DEVELOPMENT` only addresses in New York (`NY`) and California (`CA`) are supported.

# [Suggest Request](#tab/tab-ave-pitney-bowes-json-suggest-request)
```JSON
{
  "XAVRequest": {
    "AddressKeyFormat": {
      "AddressLine": [
        "1800 Main St"
      ],
      "PoliticalDivision2": "Los Angeles",
      "PoliticalDivision1": "CA",
      "PostcodePrimaryLow": "90025",
      "CountryCode": "US"
    }
  }
}
```
# [Suggest Response](#tab/tab-ave-pitney-bowes-json-suggest-response)
```JSON
{
  "addressLines": [
    "1800 S MAIN ST"
  ],
  "cityOrTown": "LOS ANGELES",
  "country": "US",
  "errors": [],
  "isResidential": false,
  "postalCode": "90015-3612",
  "stateOrProvince": "CA",
  "suggestions": [
    {
      "addressLines": [
        "1800 N MAIN ST"
      ],
      "cityOrTown": "LOS ANGELES",
      "country": "US",
      "errors": [],
      "isResidential": true,
      "postalCode": "90031-3262",
      "stateOrProvince": "CA",
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
