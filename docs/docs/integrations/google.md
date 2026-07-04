---
title: Google
---

# Google Integration

AddressValidation offers a complete integration to the [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview) to provide validation for over [39 countries](https://developers.google.com/maps/documentation/address-validation/coverage).

## Credentials

Before utilizing the integration, the setup and configuration of a [service account](https://cloud.google.com/iam/docs/service-account-overview) is required. 
It is assumed that an active project with the [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview) enabled and available.

To create the service account, you can refer to the following [article](https://cloud.google.com/iam/docs/service-accounts-create#iam-service-accounts-create-console), or if you have access to the [gcloud CLI](https://cloud.google.com/cli?hl=en) you can run the following:

```Bash
gcloud iam service-accounts create $SA_NAME \
    --description="Address Validation Service" \
    --display-name="Address Validation Service"
```

After the service account has been created, it will need to be granted a [Domain-wide Delegation](https://support.google.com/a/answer/162106?hl=en#zippy=%2Cset-up-domain-wide-delegation-for-a-client) for the scope `https://www.googleapis.com/auth/cloud-platform`. You can execute the following command to get the `oauth2ClientId` value, otherwise you can retrieve it from the [service accounts dashboard](https://console.cloud.google.com/iam-admin/serviceaccounts) under the heading `OAuth 2 Client ID`:

```Bash
gcloud iam service-accounts describe $SA_NAME@$PROJECT_ID.iam.gserviceaccount.com
```

Finally, create your [service account key](https://cloud.google.com/iam/docs/keys-create-delete#iam-service-account-keys-create-gcloud) and store it in a safe location as it will be needed later.

```Bash
gcloud iam service-accounts keys create /tmp/$SA_NAME-key.json \
    --iam-account=$SA_NAME@$PROJECT_ID.iam.gserviceaccount.com
```

> [!NOTE]
> There are no plans to support [API Key authentication](https://developers.google.com/maps/documentation/address-validation/get-api-key) at this time.

## Installation

The easiest way to install the integration into a project is through NuGet:

# [.NET CLI](#tab/tab-ave-google-cli)
```Shell
dotnet package add VisusIO.AddressValidation.Integration.Google
```
# [Package Manager](#tab/tab-ave-google-pm)
```PowerShell
Install-Package VisusIO.AddressValidation.Integration.Google
```
---

At application startup, you will need to register the integration with the [Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) container:

```csharp
builder.Services.AddGoogleAddressValidation();
```

[!INCLUDE [hybrid-cache-required](../includes/hybrid-cache-required.md)]

## Configuration

Configuration is bound from the `AddressValidationSettings:Google` section. The necessary values can be extracted from the `$SA_NAME-key.json` file produced in the [credentials](#credentials) step.

```json
{
  "AddressValidationSettings": {
    "Google": {
      "ServiceAccountEmail": "<client_email from key file>",
      "ProjectId": "<project_id from key file>",
      "PrivateKey": "<private_key from key file>",
      "ClientEnvironment": "PRODUCTION"
    }
  }
}
```

| Property | Required | Description |
|---|---|---|
| `ServiceAccountEmail` | Yes | Maps to `client_email` in the service account key file |
| `ProjectId` | Yes | Maps to `project_id` in the service account key file |
| `PrivateKey` | Yes | Maps to `private_key` in the service account key file |
| `ClientEnvironment` | No | Accepted values: `PRODUCTION`, `DEVELOPMENT`, `SANDBOX`. Defaults to `PRODUCTION` |
| `EndpointUriOverride` | SANDBOX only | Custom endpoint URI; required when `ClientEnvironment` is `SANDBOX` |
| `AuthenticationUriOverride` | SANDBOX only | Custom authentication URI; required when `ClientEnvironment` is `SANDBOX` |

> [!IMPORTANT]
> Formatting of the `PrivateKey` value **must** be preserved (newlines included).

> [!IMPORTANT]
> `PrivateKey` should be stored encrypted at rest. See the [Security](../index.md#security) for additional details.

## Standard Example

With the setup and configuration now complete, you can leverage the validator:

```csharp
public class ValidateController
{
    private readonly IAddressValidationService<GoogleAddressValidationRequest> _validationService;

    public ValidateController(IAddressValidationService<GoogleAddressValidationRequest> validationService)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GoogleAddressValidationRequest request, CancellationToken cancellationToken = default)
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
> [`EnableUspsCass`](xref:Visus.AddressValidation.Integration.Google.Models.GoogleAddressValidationRequest#Visus_AddressValidation_Integration_Google_Models_GoogleAddressValidationRequest_EnableUspsCass) is a computed property and will only be `true` if the Country is `US`.

> [!TIP]
> When re-validating an address, be sure to set the property [`PreviousResponseId`](xref:Visus.AddressValidation.Integration.Google.Models.GoogleAddressValidationRequest#Visus_AddressValidation_Integration_Google_Models_GoogleAddressValidationRequest_PreviousResponseId) value within [`GoogleAddressValidationRequest`](xref:Visus.AddressValidation.Integration.Google.Models.GoogleAddressValidationRequest). The value can be retrieved from the [`CustomResponseData`](xref:Visus.AddressValidation.Models.IAddressValidationResponse#Visus_AddressValidation_Models_IAddressValidationResponse_CustomResponseData) dictionary with the key `responseId`.

# [Request](#tab/tab-ave-google-json-request)
```JSON
{
  "address": {
    "addressLines": [
      "1600 Pennsylvania Ave NW"
    ],
    "administrativeArea": "DC",
    "locality": "Washington",
    "postalCode": "20500",
    "regionCode": "US"
  },
  "enableUspsCass": true
}
```
# [Response](#tab/tab-ave-json-response)
```JSON
{
  "addressLines": [
    "1600 PENNSYLVANIA AVE NW"
  ],
  "cityOrTown": "WASHINGTON",
  "country": "US",
  "customResponseData": {
    "addressRecordType": "S",
    "carrierRoute": "C000",
    "carrierRouteIndicator": "D",
    "cassProcessed": true,
    "county": "DISTRICT OF COLUMBIA",
    "deliveryPointCheckDigit": "0",
    "deliveryPointCode": "00",
    "dpvCmra": "N",
    "dpvConfirmation": "Y",
    "dpvDoorNotAccessible": "N",
    "dpvDrop": "N",
    "dpvEnhancedDeliveryCode": "Y",
    "dpvFootnote": "AABB",
    "dpvNonDeliveryDays": "N",
    "dpvNoSecureLocation": "N",
    "dpvNoStat": "N",
    "dpvPbsa": "N",
    "dpvThrowback": "N",
    "dpvVacant": "N",
    "elotFlag": "A",
    "elotNumber": "0001",
    "fipsCountyCode": "001",
    "googlePlaceId": "ChIJ37HL3ry3t4kRv3YLyiMEoGg",
    "latitude": 38.8976763,
    "longitude": -77.0365298,
    "postOfficeCity": "WASHINGTON",
    "postOfficeState": "DC",
    "responseId": "c3d4e5f6-a7b8-9012-cdef-34567890abcd"
  },
  "errors": [],
  "isResidential": false,
  "postalCode": "20500-0003",
  "stateOrProvince": "DC",
  "suggestions": [],
  "warnings": []
}
```
---

[!INCLUDE [is-residential-note](../includes/is-residential-note.md)]

> [!NOTE]
> The properties `googlePlaceId`, `latitude`, `longitude`, and `responseId` will always be present in `customResponseData`. If USPS&reg; CASS&trade; is supported for the destination (currently only the `US`), then those properties will be present within `customResponseData`.

> [!NOTE]
> The `Suggestions` collection will always be empty for responses returned as the [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview)
> does not provide address suggestions.

[!INCLUDE [internal-validation-note](../includes/internal-validation-note.md)]