---
title: Google
---

# Google Integration

AddressValidation offers a complete integration to the [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview) to provide validation for over [39 countries](https://developers.google.com/maps/documentation/address-validation/coverage).

[!INCLUDE [batch-validation-not-supported-note](../includes/batch-validation-not-supported-note.md)]

## Credentials

Set up a [service account](https://cloud.google.com/iam/docs/service-account-overview) before you use this integration. You need an active project with the [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview) enabled.

To create the service account, follow this [article](https://cloud.google.com/iam/docs/service-accounts-create#iam-service-accounts-create-console). If you have access to the [gcloud CLI](https://cloud.google.com/cli?hl=en), run the following commands instead:

# [Shell](#tab/tab-ave-google-gcloud-shell)
```bash
gcloud iam service-accounts create $SA_NAME \
    --description="Address Validation Service" \
    --display-name="Address Validation Service"
```
# [PowerShell](#tab/tab-ave-google-gcloud-powershell)
```PowerShell
gcloud iam service-accounts create $SA_NAME `
    --description="Address Validation Service" `
    --display-name="Address Validation Service"
```
---

After you create the service account, grant it [Domain-wide Delegation](https://support.google.com/a/answer/162106?hl=en#zippy=%2Cset-up-domain-wide-delegation-for-a-client) for the scope `https://www.googleapis.com/auth/cloud-platform`. Run the following command to get the `oauth2ClientId` value. You can also find it on the [service accounts dashboard](https://console.cloud.google.com/iam-admin/serviceaccounts), under the heading `OAuth 2 Client ID`:

# [Shell](#tab/tab-ave-google-gcloud-shell)
```bash
gcloud iam service-accounts describe $SA_NAME@$PROJECT_ID.iam.gserviceaccount.com
```
# [PowerShell](#tab/tab-ave-google-gcloud-powershell)
```PowerShell
gcloud iam service-accounts describe $SA_NAME@$PROJECT_ID.iam.gserviceaccount.com
```
---

Finally, create your [service account key](https://cloud.google.com/iam/docs/keys-create-delete#iam-service-account-keys-create-gcloud). Store it in a safe location — you need it later.

# [Shell](#tab/tab-ave-google-gcloud-shell)
```bash
gcloud iam service-accounts keys create /tmp/$SA_NAME-key.json \
    --iam-account=$SA_NAME@$PROJECT_ID.iam.gserviceaccount.com
```
# [PowerShell](#tab/tab-ave-google-gcloud-powershell)
```PowerShell
gcloud iam service-accounts keys create $env:TEMP\$SA_NAME-key.json `
    --iam-account=$SA_NAME@$PROJECT_ID.iam.gserviceaccount.com
```
---

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

Register the integration with the [Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) container at application startup:

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
> Preserve the formatting of the `PrivateKey` value, including newlines.

> [!IMPORTANT]
> Store `PrivateKey` encrypted at rest. See [Security](../index.md#security) for more details.

## Standard Example

After you complete setup and configuration, use the validator:

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
> [`EnableUspsCass`](xref:Visus.AddressValidation.Integration.Google.Models.GoogleAddressValidationRequest#Visus_AddressValidation_Integration_Google_Models_GoogleAddressValidationRequest_EnableUspsCass) is a computed property. It is `true` only when Country is `US`.

> [!TIP]
> When you revalidate an address, set [`PreviousResponseId`](xref:Visus.AddressValidation.Integration.Google.Models.GoogleAddressValidationRequest#Visus_AddressValidation_Integration_Google_Models_GoogleAddressValidationRequest_PreviousResponseId) on [`GoogleAddressValidationRequest`](xref:Visus.AddressValidation.Integration.Google.Models.GoogleAddressValidationRequest). Get this value from the [`CustomResponseData`](xref:Visus.AddressValidation.Models.IAddressValidationResponse#Visus_AddressValidation_Models_IAddressValidationResponse_CustomResponseData) dictionary, under the key `responseId`.

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
> The properties `googlePlaceId`, `latitude`, `longitude`, and `responseId` are always present in `customResponseData`. When USPS&reg; CASS&trade; is supported for the destination (currently only `US`), those properties are present in `customResponseData`.

> [!NOTE]
> The `Suggestions` collection is always empty. The [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview) does not provide address suggestions.

[!INCLUDE [internal-validation-note](../includes/internal-validation-note.md)]