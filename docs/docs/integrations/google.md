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
dotnet package add Visus.AddressValidation.Integration.Google
```
# [Package Manager](#tab/tab-ave-google-pm)
```PowerShell
Install-Package Visus.AddressValidation.Integration.Google
```
---

At application startup, you will need to register the integration with the [Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) container:

```csharp
builder.Services.AddGoogleAddressValidation();
```

[!INCLUDE [distributed-cache-required](../includes/distributed-cache-required.md)]

## Configuration

Configuration of the integration is relatively straight forward and **all** parameters are read through an `IConfiguration` instance.
The necessary configuration values can be extracted from the `$SA_NAME-key.json` file that was produced earlier in the [credentials](#credentials) step.

| Key                                      | JSON Field   |
|------------------------------------------|--------------|
| `VS_AVE_GCP_SERVICE_ACCOUNT_EMAIL`       | client_email |
| `VS_AVE_GCP_PROJECT_ID`                  | project_id   |
| `VS_AVE_GCP_SERVICE_ACCOUNT_PRIVATE_KEY` | private_key  |

> [!IMPORTANT]
> Formatting of the `private_key` value **must** be preserved.

> [!IMPORTANT]
> `VS_AVE_GCP_SERVICE_ACCOUNT_PRIVATE_KEY` should be stored encrypted at rest. See the [Security](../index.md#security) for additional details.

## Standard Example

With the setup and configuration now complete, you can leverage the validator:

```csharp
public class ValidateController(IAddressValidationService<GoogleAddressValidationRequest> validationService)
{
    private readonly IAddressValidationService<GoogleAddressValidationRequest> _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    
    [HttpGet]
    public async ValueTask<IActionResult> Get()
    {
        new GoogleAddressValidationRequest
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

> [!NOTE]
> [`EnableUspsCass`](xref:Visus.AddressValidation.Integration.Google.Http.GoogleAddressValidationRequest#Visus_AddressValidation_Integration_Google_Http_GoogleAddressValidationRequest_EnableUspsCass) is a computed property and will only be `true` if the Country is `US`.

> [!TIP]
> When re-validating an address, be sure to set the property [`PreviousResponseId`](xref:Visus.AddressValidation.Integration.Google.Http.GoogleAddressValidationRequest#Visus_AddressValidation_Integration_Google_Http_GoogleAddressValidationRequest_PreviousResponseId) value within [`GoogleAddressValidationRequest`](xref:Visus.AddressValidation.Integration.Google.Http.GoogleAddressValidationRequest). The value can be retrieved from the [`CustomResponseData`](xref:Visus.AddressValidation.Model.IAddressValidationResponse#Visus_AddressValidation_Model_IAddressValidationResponse_CustomResponseData) dictionary with the key `responseId`.

# [Request](#tab/tab-ave-google-json-request)
```JSON
{
  "address": {
    "addressLines": [
      "1600 Amphitheatre Pkwy"
    ],
    "administrativeArea": "CA",
    "locality": "Mountain View",
    "postalCode": "94043",
    "regionCode": "US"
  },
  "enableUspsCass": true
}
```
# [Response](#tab/tab-ave-json-response)
```JSON
{
  "addressLines": [
    "1600 AMPHITHEATRE PKWY"
  ],
  "cityOrTown": "MOUNTAIN VIEW",
  "country": "US",
  "customResponseData": {
    "addressRecordType": "S",
    "carrierRoute": "C909",
    "carrierRouteIndicator": "D",
    "cassProcessed": true,
    "county": "SANTA CLARA",
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
    "dpvNoStat": "Y",
    "dpvNoStatReasonCode": 5,
    "dpvPbsa": "N",
    "dpvThrowback": "N",
    "dpvVacant": "N",
    "elotFlag": "A",
    "elotNumber": "0103",
    "fipsCountyCode": "085",
    "googlePlaceId": "ChIJF4Yf2Ry7j4AR__1AkytDyAE",
    "latitude": 37.4215939,
    "longitude": -122.0845152,
    "postOfficeCity": "MOUNTAIN VIEW",
    "postOfficeState": "CA",
    "responseId": "b951c4fe-2c9f-4222-a539-1cafd8348a42"
  },
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

> [!NOTE]
> The properties `googlePlaceId`, `latitude`, `longitude`, and `responseId` will always be present in `customResponseData`. If USPS&reg; CASS&trade; is supported for the destination (currently only the `US`), then those properties will be present within `customResponseData`.

> [!NOTE]
> The `Suggestions` collection will always be empty for responses returned as the [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview)
> does not provide address suggestions.

[!INCLUDE [internal-validation-note](../includes/internal-validation-note.md)]