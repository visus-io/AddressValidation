namespace Visus.AddressValidation.Integration.FedEx;

using AddressValidation.Abstractions;

/// <summary>
///     Configuration constants for FedEx Service Integration
/// </summary>
public static class Constants
{
    /// <summary>
    ///     FedEx Account Number Configuration Key
    /// </summary>
    public const string AccountNumberConfigurationKey = "VS_AVE_FDX_ACCOUNT_NUMBER";

    /// <summary>
    ///     Client Environment Configuration Key
    /// </summary>
    public const string ClientEnvironmentConfigurationKey = "VS_AVE_FDX_CLIENT_ENVIRONMENT";

    /// <summary>
    ///     OAuth2 Client ID Configuration Key
    /// </summary>
    public const string ClientIdConfigurationKey = "VS_AVE_FDX_CLIENT_ID";

    /// <summary>
    ///     OAuth2 Client Secret Configuration Key
    /// </summary>
    public const string ClientSecretConfigurationKey = "VS_AVE_FDX_CLIENT_SECRET";

    /// <summary>
    ///     FedEx API Development Endpoint
    /// </summary>
    public static readonly Uri DevelopmentEndpointBaseUri = new("https://apis-sandbox.fedex.com");

    /// <summary>
    ///     FedEx API Production Endpoint
    /// </summary>
    public static readonly Uri ProductionEndpointBaseUri = new("https://apis.fedex.com");

    /// <summary>
    ///     Countries that are supported by the FedEx Address Validation API.
    /// </summary>
    public static readonly IReadOnlySet<CountryCode> SupportedCountries = new HashSet<CountryCode>
    {
        CountryCode.AR,
        CountryCode.AT,
        CountryCode.AU,
        CountryCode.AW,
        CountryCode.BB,
        CountryCode.BE,
        CountryCode.BR,
        CountryCode.BS,
        CountryCode.CA,
        CountryCode.CH,
        CountryCode.CL,
        CountryCode.CO,
        CountryCode.CR,
        CountryCode.CZ,
        CountryCode.DE,
        CountryCode.DK,
        CountryCode.DO,
        CountryCode.EE,
        CountryCode.ES,
        CountryCode.FI,
        CountryCode.FR,
        CountryCode.GB,
        CountryCode.GR,
        CountryCode.GT,
        CountryCode.HK,
        CountryCode.IT,
        CountryCode.JM,
        CountryCode.KY,
        CountryCode.MX,
        CountryCode.MY,
        CountryCode.NL,
        CountryCode.NO,
        CountryCode.NZ,
        CountryCode.PA,
        CountryCode.PE,
        CountryCode.PT,
        CountryCode.SE,
        CountryCode.SG,
        CountryCode.TT,
        CountryCode.US,
        CountryCode.UY,
        CountryCode.VE,
        CountryCode.VI,
        CountryCode.ZA,
    };
}
