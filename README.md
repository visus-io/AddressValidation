# AddressValidation

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/visus-io/AddressValidation/ci.yml?style=for-the-badge&logo=github)
![Sonar Quality Gate](https://img.shields.io/sonar/quality_gate/visus%3AAddressValidation?server=https%3A%2F%2Fsonarcloud.io&style=for-the-badge&logo=sonar)
![Sonar Coverage](https://img.shields.io/sonar/coverage/visus%3AAddressValidation?server=https%3A%2F%2Fsonarcloud.io&style=for-the-badge&logo=sonar)

![Static Badge](https://img.shields.io/badge/license-mit-green?style=for-the-badge)

| Package | Version | Downloads |
|---|---|---|
| `Visus.AddressValidation.Integration.FedEx` | [![NuGet Version](https://img.shields.io/nuget/v/Visus.AddressValidation.Integration.FedEx?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Visus.AddressValidation.Integration.FedEx) | [![NuGet Downloads](https://img.shields.io/nuget/dt/Visus.AddressValidation.Integration.FedEx?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Visus.AddressValidation.Integration.FedEx) |
| `Visus.AddressValidation.Integration.Google` | [![NuGet Version](https://img.shields.io/nuget/v/Visus.AddressValidation.Integration.Google?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Visus.AddressValidation.Integration.Google) | [![NuGet Downloads](https://img.shields.io/nuget/dt/Visus.AddressValidation.Integration.Google?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Visus.AddressValidation.Integration.Google) |
| `Visus.AddressValidation.Integration.PitneyBowes` | [![NuGet Version](https://img.shields.io/nuget/v/Visus.AddressValidation.Integration.PitneyBowes?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Visus.AddressValidation.Integration.PitneyBowes) | [![NuGet Downloads](https://img.shields.io/nuget/dt/Visus.AddressValidation.Integration.PitneyBowes?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Visus.AddressValidation.Integration.PitneyBowes) |
| `Visus.AddressValidation.Integration.Ups` | [![NuGet Version](https://img.shields.io/nuget/v/Visus.AddressValidation.Integration.Ups?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Visus.AddressValidation.Integration.Ups) | [![NuGet Downloads](https://img.shields.io/nuget/dt/Visus.AddressValidation.Integration.Ups?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Visus.AddressValidation.Integration.Ups) |

AddressValidation is a .NET library that provides a unified, provider-agnostic API for validating physical mailing addresses.

> [!NOTE]
> AddressValidation supports [trimming](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming) and [native AOT deployments](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/).

## Integrations

| Service | Coverage | Status |
|---|---|---|
| [FedEx&reg; Address Validation API](https://developer.fedex.com/api/en-us/catalog/address-validation.html) | [46 countries](https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html) | ![Status](https://img.shields.io/badge/status-complete-green?style=flat-square) |
| [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview) | [39 countries](https://developers.google.com/maps/documentation/address-validation/coverage) | ![Status](https://img.shields.io/badge/status-complete-green?style=flat-square) |
| [Pitney Bowes Address Validation API](https://docs.shippingapi.pitneybowes.com/address-validation.html) | United States | ![Status](https://img.shields.io/badge/status-complete-green?style=flat-square) |
| [UPS&reg; Address Validation API](https://developer.ups.com/api/reference) | United States | ![Status](https://img.shields.io/badge/status-complete-green?style=flat-square) |
| [USPS&reg; Address Validation API](https://developer.usps.com/api/93) | United States | ![Status](https://img.shields.io/badge/status-planned-blue?style=flat-square) |

## Quick Start

### Installation

Install the integration package for your chosen provider via the .NET CLI:

```shell
dotnet package add Visus.AddressValidation.Integration.FedEx
```

### Registration

All integrations require [`HybridCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.hybrid.hybridcache) for OAuth token caching. Register it alongside the integration at application startup:

```csharp
builder.Services.AddHybridCache();
builder.Services.AddFedExAddressValidation();
```

### Configuration

Configuration is read from `appsettings.json` under the `AddressValidationSettings:<Provider>` section. For FedEx:

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
| `ClientId` | Yes | OAuth 2.0 client ID |
| `ClientSecret` | Yes | OAuth 2.0 client secret |
| `ClientEnvironment` | No | `PRODUCTION`, `DEVELOPMENT`, or `SANDBOX`. Defaults to `DEVELOPMENT` |

> [!IMPORTANT]
> `ClientId` and `ClientSecret` should be stored encrypted at rest. See [Security](https://ave.projects.visus.io/docs/index.html#security) in the documentation for recommended approaches.

Each provider has its own set of configuration properties. See the [documentation](https://ave.projects.visus.io/) for the full reference.

### Usage

Inject `IAddressValidationService<TRequest>` into your service or controller and call `ValidateAsync`:

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

Each integration package exposes its own `TRequest` type (e.g., `FedExAddressValidationRequest`, `GoogleAddressValidationRequest`) and a corresponding `Add*AddressValidation()` extension method. See the [documentation](https://ave.projects.visus.io/) for provider-specific options.

## Documentation

Full documentation is available at [https://ave.projects.visus.io/](https://ave.projects.visus.io/).
