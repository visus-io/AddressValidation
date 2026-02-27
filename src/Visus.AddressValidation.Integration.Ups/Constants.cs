namespace Visus.AddressValidation.Integration.Ups;

using AddressValidation.Abstractions;

/// <summary>
///     Configuration Constants for UPS Service Integration
/// </summary>
public static class Constants
{
    /// <summary>
    ///     UPS Account Number Configuration Key
    /// </summary>
    public const string AccountNumberConfigurationKey = "VS_AVE_UPS_ACCOUNT_NUMBER";

    /// <summary>
    ///     Client Environment Configuration Key
    /// </summary>
    public const string ClientEnvironmentConfigurationKey = "VS_AVE_UPS_CLIENT_ENVIRONMENT";

    /// <summary>
    ///     OAuth2 Client ID Configuration Key
    /// </summary>
    public const string ClientIdConfigurationKey = "VS_AVE_UPS_CLIENT_ID";

    /// <summary>
    ///     OAuth2 Client Secret Configuration Key
    /// </summary>
    public const string ClientSecretConfigurationKey = "VS_AVE_UPS_CLIENT_SECRET";

    /// <summary>
    ///     UPS API Development Endpoint
    /// </summary>
    public static readonly Uri DevelopmentEndpointBaseUri = new("https://wwwcie.ups.com");

    /// <summary>
    ///     UPS API Production Endpoint
    /// </summary>
    public static readonly Uri ProductionEndpointBaseUri = new("https://onlinetools.ups.com");

    /// <summary>
    ///     Countries that are supported by the UPS Address Validation API.
    /// </summary>
    public static readonly IReadOnlySet<CountryCode> SupportedCountries = new HashSet<CountryCode>
    {
        CountryCode.US,
        CountryCode.PR,
    };
}
