---
title: FedEx
---

# FedEx&reg; Integration

AddressValidation offers a complete integration to the [FedEx&reg; Address Validation API](https://developer.fedex.com/api/en-us/catalog/address-validation.html) to provide validation for [46 countries](https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html).

## Credentials

Before utilizing the integration, you will need a [developer account](https://developer.fedex.com/) with FedEx. After you have signed in to the account, [follow these instructions](https://developer.fedex.com/api/en-us/get-started.html) to obtain your credentials.

## Installation

The easiest way to install the integration into a project is through NuGet:

# [.NET CLI](#tab/tab-ave-fedex-cli)
```Shell
dotnet package add VisusIO.AddressValidation.Integration.FedEx
```
# [Package Manager](#tab/tab-ave-fedex-pm)
```PowerShell
Install-Package VisusIO.AddressValidation.Integration.FedEx
```
---

At application startup, you will need to register the integration with the [Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) container:

```csharp
builder.Services.AddFedExAddressValidation();
```

[!INCLUDE [hybrid-cache-required](../includes/hybrid-cache-required.md)]

## Configuration

Configuration is bound from the `AddressValidationSettings:FedEx` section.

```json
{
  "AddressValidationSettings": {
    "FedEx": {
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
| `AccountNumber` | Yes | Your FedEx account number |
| `ClientId` | Yes | OAuth 2.0 client ID issued by FedEx for your registered application |
| `ClientSecret` | Yes | OAuth 2.0 client secret issued by FedEx for your registered application |
| `ClientEnvironment` | No | Accepted values: `PRODUCTION`, `DEVELOPMENT`, `SANDBOX`. Defaults to `DEVELOPMENT` |
| `Locale` | No | [IETF BCP 47](https://www.rfc-editor.org/info/bcp47) language tag for the response locale (e.g., `en-US`). When omitted, FedEx uses its default locale. |
| `EndpointUriOverride` | SANDBOX only | Custom endpoint URI; required when `ClientEnvironment` is `SANDBOX` |

> [!IMPORTANT]
> `ClientId` and `ClientSecret` should be stored encrypted at rest. See the [Security](../index.md#security) for additional details.

## Standard Example

The following example demonstrates a standard address validation request.

```csharp
public class ValidateController
{
    private readonly IAddressValidationService<FedExAddressValidationRequest> _validationService;

    public ValidateController(IAddressValidationService<FedExAddressValidationRequest> validationService)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FedExAddressValidationRequest request, CancellationToken cancellationToken = default)
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

# [Request](#tab/tab-ave-fedex-json-request)
```JSON
{
  "addressesToValidate": [
    {
      "address": {
        "streetLines": [
          "1000 5th Ave"
        ],
        "city": "New York",
        "stateOrProvince": "NY",
        "postalCode": "10028",
        "countryCode": "US"
      }
    }
  ]
}
```
# [Response](#tab/tab-ave-fedex-json-response)
```JSON
{
  "addressLines": [
    "1000 5TH AVE"
  ],
  "cityOrTown": "NEW YORK",
  "country": "US",
  "customResponseData": {
    "transactionId": "APIF_SV_ADVC_TxID7a1b2c3d-4e5f-6789-abcd-ef0123456789",
    "addressPrecision": "DELIVERY_POINT",
    "addressType": "STANDARDIZED",
    "containsMultipleUnits": false,
    "countrySupported": true,
    "dataVintage": "2025-01-01",
    "generalDelivery": false,
    "inserted": false,
    "isBaseAddressForMultiUnit": false,
    "isDeliveryPointValid": true,
    "isPostOfficeBox": false,
    "isPostOfficeBoxOnlyZip": false,
    "isRuralRouteConversion": false,
    "isUniquePostalCode": false,
    "matched": true,
    "matchSource": "Postal",
    "normalizedStatusNameDpv": true,
    "postOfficeBox": false,
    "resolutionInput": "RAW_ADDRESS",
    "resolutionMethod": "USPS_VALIDATE",
    "resolved": true,
    "ruralRouteHighwayContract": false,
    "splitZip": false,
    "standardizedStatusNameMatchSource": "Postal",
    "validlyFormed": true
  },
  "errors": [],
  "isResidential": false,
  "postalCode": "10028-0198",
  "stateOrProvince": "NY",
  "suggestions": [],
  "warnings": []
}
```
---

> [!NOTE]
> `transactionId` will always be present in `customResponseData`. When [`CustomerTransactionId`](xref:Visus.AddressValidation.Integration.FedEx.Models.FedExAddressValidationRequest#Visus_AddressValidation_Integration_FedEx_Models_FedExAddressValidationRequest_CustomerTransactionId) is set on the request, `customerTransactionId` will also be present with the same value echoed back by the FedEx API.

> [!NOTE]
> The `Suggestions` collection will always be empty as the [FedEx&reg; Address Validation API](https://developer.fedex.com/api/en-us/catalog/address-validation.html) does not provide address suggestions.

[!INCLUDE [is-residential-note](../includes/is-residential-note.md)]

[!INCLUDE [internal-validation-note](../includes/internal-validation-note.md)]
