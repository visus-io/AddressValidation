---
title: Introduction
---
# AddressValidation

AddressValidation is a .NET library with the goal of providing a simple and streamlined process of validating physical addresses.

> [!NOTE]
> AddressValidation supports [trimming](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming) and [native AOT deployments](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/).

## Integrations

AddressValidation comes with several service integrations pre-built and ready to use. Reference the table below for details on available integrations.

| Service                                                                                                       | Integration                                  | Coverage                                                                                      | Status       |
|---------------------------------------------------------------------------------------------------------------|----------------------------------------------|-----------------------------------------------------------------------------------------------|--------------|
| [FedEx&reg; Address Validation API](https://developer.fedex.com/api/en-us/catalog/address-validation.html)    | [FedEx](integrations/fedex.md)               | [46 countries](https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html) | **Complete** |
| [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview) | [Google](integrations/google.md)             | [39 countries](https://developers.google.com/maps/documentation/address-validation/coverage)  | **Complete** |
| [Pitney Bowes Address Validation API](https://docs.shippingapi.pitneybowes.com/address-validation.html)       | [Pitney Bowes](integrations/pitney-bowes.md) | United States                                                                                 | **Complete** |
| [UPS&reg; Address Validation API](https://developer.ups.com/api/reference)                                    | [UPS](integrations/ups.md)                   | United States                                                                                 | **Complete** |
| [USPS&reg; Address Validation API](https://developer.usps.com/api/93)                                        |                                              | United States                                                                                 | *Planned*    |

If there is no integration for a service you wish to you use, you can either open a [feature request](https://github.com/visus-io/AddressValidation/issues/new?template=feature_request.yml) or you can read on how to develop a [custom integration](integrations/custom/introduction.md).

## Batch Validation

[`IBatchAddressValidationService<TRequest>`](xref:Visus.AddressValidation.Services.IBatchAddressValidationService`1) is an opt-in interface for integrations whose provider API natively supports validating multiple addresses in a single call. Where available, it lets you submit a list of requests and get back a positionally-aligned list of responses instead of issuing one call per address.

[FedEx](integrations/fedex.md#batch-example) is currently the only integration that implements it. If you are building a [custom integration](integrations/custom/introduction.md) against a provider that supports multi-address requests, see the [Batch Validation](integrations/custom/registering-services.md#batch-validation-service-optional) section of the custom integration guide.

## Caching

To provide maximum performance all integrations within AddressValidation require [`HybridCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.hybrid.hybridcache) to cache authentication tokens. Register it at application startup:

```csharp
builder.Services.AddHybridCache();
```

By default, `HybridCache` uses an in-process (L1) cache, which is sufficient for single-server deployments, local development, and testing environments. For multi-server deployments, also register an [`IDistributedCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache) provider to serve as the distributed (L2) cache layer. Refer to the [documentation](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed) for available providers.

> [!NOTE]
> Currently only [access tokens](https://oauth.net/2/access-tokens/) are cached. Caching of requests and responses is not natively supported for two reasons:
> - **Privacy regulations:** address data constitutes personally identifiable information (PII) subject to [GDPR](https://gdpr-info.eu/) and [CCPA](https://oag.ca.gov/privacy/ccpa).
> - **Provider Terms of Service:** Google, Pitney Bowes, and UPS all restrict caching of API responses. Google and Pitney Bowes both permit temporary caching for up to 30 days under specific conditions (user consent, secure storage, no cross-user reuse); UPS prohibits all uses of response data not explicitly permitted by their agreement.

## Security

> [!IMPORTANT]
> AddressValidation **does not** (and will not) provide a facility for encrypting or decrypting sensitive information so please read this section carefully.

The pre-built integrations provided by AddressValidation all make use of an `IConfiguration` instance to read their respective configuration values.
As a result, many of the configuration values will contain sensitive information such as client secrets and will need to be secured properly.

Fortunately, there are several options available to ensure that this information is stored securely:

- [Azure AppConfig with Azure KeyVault](https://learn.microsoft.com/en-us/samples/azure/azure-sdk-for-net/app-secrets-configuration)
- [Custom Configuration Provider with AWS Secrets Manager](https://aws.amazon.com/blogs/modernizing-with-aws/how-to-load-net-configuration-from-aws-secrets-manager/)
- [Custom Configuration Provider with Google Secrets Manager](https://www.nuget.org/packages/Gcp.SecretManager.Provider)

For any other implementation a [custom configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/custom-configuration-provider) will be required.

## Instrumentation

All integrations emit OpenTelemetry-compatible traces and metrics via `System.Diagnostics`, covering both address validation calls and OAuth token fetches/cache results. See [Instrumentation](instrumentation.md) for setup, the full list of activities and metrics, and export examples for popular observability backends.