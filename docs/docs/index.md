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
| [FedEx&reg; Address Validation API](https://developer.fedex.com/api/en-us/catalog/address-validation.html)    |                                              | [46 countries](https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html) | *Planned*    |
| [Google Address Validation API](https://developers.google.com/maps/documentation/address-validation/overview) | [Google](integrations/google.md)             | [39 countries](https://developers.google.com/maps/documentation/address-validation/coverage)  | **Complete** |
| [Pitney Bowes Address Validation API](https://docs.shippingapi.pitneybowes.com/address-validation.html)       | [Pitney Bowes](integrations/pitney-bowes.md) | United States                                                                                 | **Complete** |
| [UPS&reg; Address Validation API](https://developer.ups.com/api/reference)                                    | [UPS](integrations/ups.md)                   | United States                                                                                 | **Complete** |

> [!NOTE]
> Due to the inability to redact query arguments in HttpClient logging (see [dotnet/runtime#68675](https://github.com/dotnet/runtime/issues/68675) and [dotnet/runtime#77312](https://github.com/dotnet/runtime/issues/77312)), there are no plans to build a service integration for the [USPS&reg; Address Validation API](https://developer.usps.com/api/93) at this time. 
> 
> If support for [USPS&reg; CASS&trade;](https://postalpro.usps.com/certifications/cass) is required, then the [Google](integrations/google.md) integration is recommended.

If there is no integration for a service you wish to you use, you can either open a [feature request](#) or you can read on how to develop a [custom integration](integrations/custom/introduction.md).

## Caching

To provide maximum performance all integrations within AddressValidation require an instance of [IDistributedCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache) to cache operations.

For single-server deployments, local development, and testing environments, the [distributed memory cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed#distributed-memory-cache) provider is sufficient. For all other use cases, refer to the [documentation](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed) on additional providers and solutions.

> [!NOTE]
> Currently only [access tokens](https://oauth.net/2/access-tokens/) are cached. Caching of requests and responses is not natively supported due to [GDPR](https://gdpr-info.eu/) and [CCPA](https://oag.ca.gov/privacy/ccpa) concerns.

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

---

The **AddressValidation.Demo** project contains a barebones [ConfigurationProvider](#) implementation that utilizes the [Data Protection API](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction) and can be used as a reference.

> [!WARNING]
> Take care when using the [Data Protection API](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction) and ensure that the key is [stored](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers) securely.