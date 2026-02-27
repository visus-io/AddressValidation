namespace Visus.AddressValidation.Integration.PitneyBowes;

using AddressValidation.Abstractions;

/// <summary>
///     Configuration Constants for Pitney Bowes Service Integration
/// </summary>
public static class Constants
{
    /// <summary>
    ///     Pitney Bowes API Key Configuration Key
    /// </summary>
    public const string ApiKeyConfigurationKey = "VS_AVE_PB_API_KEY";

    /// <summary>
    ///     Pitney Bowes API Secret Configuration Key
    /// </summary>
    public const string ApiSecretConfigurationKey = "VS_AVE_PB_API_SECRET";

    /// <summary>
    ///     Client Environment Configuration Key
    /// </summary>
    public const string ClientEnvironmentConfigurationKey = "VS_AVE_PB_CLIENT_ENVIRONMENT";

    /// <summary>
    ///     Pitney Bowes Developer ID Configuration Key
    /// </summary>
    public const string DeveloperIdConfigurationKey = "VS_AVE_PB_DEVELOPER_ID";

    /// <summary>
    ///     Pitney Bowes Development Endpoint
    /// </summary>
    public static readonly Uri DevelopmentEndpointBaseUri = new("https://shipping-api-sandbox.pitneybowes.com/");

    /// <summary>
    ///     Pitney Bowes Production Endpoint
    /// </summary>
    public static readonly Uri ProductionEndpointBaseUri = new("https://shipping-api.pitneybowes.com/");

    /// <summary>
    ///     Countries that are supported by the Pitney Bowes Address Validation API.
    /// </summary>
    public static readonly IReadOnlySet<CountryCode> SupportedCountries = new HashSet<CountryCode>
    {
        CountryCode.US,
    };
}
