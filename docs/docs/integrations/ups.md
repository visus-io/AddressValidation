---
title: UPS
---

# UPS&reg; Integration

AddressValidation offers a complete integration to the [UPS&reg; Address Validation API](https://developer.ups.com/api/reference) to provide validation for the United States and Puerto Rico.

[!INCLUDE [batch-validation-not-supported-note](../includes/batch-validation-not-supported-note.md)]

## Credentials

Before utilizing the integration, you will need a [developer account](https://developer.ups.com/) along with an active UPS account. After you have signed in to the account, [follow these instructions](https://developer.ups.com/get-started) to obtain your credentials.

## Installation

The easiest way to install the integration into a project is through NuGet:

# [.NET CLI](#tab/tab-ave-pitney-bowes-cli)
```Shell
dotnet package add VisusIO.AddressValidation.Integration.Ups
```
# [Package Manager](#tab/tab-ave-pitney-bowes-pm)
```PowerShell
Install-Package VisusIO.AddressValidation.Integration.Ups
```
---

At application startup, you will need to register the integration with the [Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) container:

```csharp
builder.Services.AddUpsAddressValidation();
```

[!INCLUDE [hybrid-cache-required](../includes/hybrid-cache-required.md)]

## Configuration

Configuration is bound from the `AddressValidationSettings:Ups` section.

```json
{
  "AddressValidationSettings": {
    "Ups": {
      "AccountNumber": "<your account number>",
      "ClientId": "<your client id>",
      "ClientSecret": "<your client secret>",
      "ClientEnvironment": "PRODUCTION"
    }
  }
}
```

| Property | Required | Description |
|---|---|---|
| `AccountNumber` | Yes | Your UPS account number |
| `ClientId` | Yes | OAuth 2.0 client ID issued by UPS for your registered application |
| `ClientSecret` | Yes | OAuth 2.0 client secret issued by UPS for your registered application |
| `ClientEnvironment` | No | Accepted values: `PRODUCTION`, `DEVELOPMENT`, `SANDBOX`. Defaults to `DEVELOPMENT` |
| `EndpointUriOverride` | SANDBOX only | Custom endpoint URI; required when `ClientEnvironment` is `SANDBOX` |

> [!IMPORTANT]
> `ClientId` and `ClientSecret` should be stored encrypted at rest. See the [Security](../index.md#security) for additional details.

## Standard Example

The following example demonstrates a standard address validation request.

```csharp
public class ValidateController
{
    private readonly IAddressValidationService<UpsAddressValidationRequest> _validationService;

    public ValidateController(IAddressValidationService<UpsAddressValidationRequest> validationService)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UpsAddressValidationRequest request, CancellationToken cancellationToken = default)
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

> [!NOTE]
> When `ClientEnvironment` is set to `DEVELOPMENT` only addresses in New York (`NY`) and California (`CA`) are supported.

# [Request](#tab/tab-ave-pitney-bowes-json-request)
```JSON
{
  "XAVRequest": {
    "Request": {
      "RequestOption": "3"
    },
    "AddressKeyFormat": {
      "AddressLine": [
        "1 Infinite Loop"
      ],
      "PoliticalDivision2": "Cupertino",
      "PoliticalDivision1": "CA",
      "PostcodePrimaryLow": "95014",
      "CountryCode": "US"
    }
  }
}
```
# [Response](#tab/tab-ave-pitney-bowes-json-response)
```JSON
{
  "addressLines": [
    "1 INFINITE LOOP"
  ],
  "cityOrTown": "CUPERTINO",
  "country": "US",
  "errors": [],
  "isResidential": false,
  "postalCode": "95014-2083",
  "stateOrProvince": "CA",
  "suggestions": [],
  "warnings": []
}
```
---

## Suggestion Example

In the event of an incomplete or ambiguous request, a potential match along with suggestions may be returned.

> [!NOTE]
> When `ClientEnvironment` is set to `DEVELOPMENT` only addresses in New York (`NY`) and California (`CA`) are supported.

# [Suggest Request](#tab/tab-ave-pitney-bowes-json-suggest-request)
```JSON
{
  "XAVRequest": {
    "Request": {
      "RequestOption": "3"
    },
    "AddressKeyFormat": {
      "AddressLine": [
        "1 Infinite Lp"
      ],
      "PoliticalDivision2": "Cupertino",
      "PoliticalDivision1": "CA",
      "PostcodePrimaryLow": "95014",
      "CountryCode": "US"
    }
  }
}
```
# [Suggest Response](#tab/tab-ave-pitney-bowes-json-suggest-response)
```JSON
{
  "addressLines": [
    "1 INFINITE LP"
  ],
  "cityOrTown": "CUPERTINO",
  "country": "US",
  "errors": [],
  "isResidential": false,
  "postalCode": "95014",
  "stateOrProvince": "CA",
  "suggestions": [
    {
      "addressLines": [
        "1 INFINITE LOOP"
      ],
      "cityOrTown": "CUPERTINO",
      "country": "US",
      "errors": [],
      "isResidential": false,
      "postalCode": "95014-2083",
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
