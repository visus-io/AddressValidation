namespace Visus.AddressValidation.Integration.Google;

using AddressValidation.Abstractions;

/// <summary>
///     Configuration Constants for Google Service Integration
/// </summary>
public static class Constants
{
    /// <summary>
    ///     Service Account Private Key
    /// </summary>
    public const string PrivateKeyConfigurationKey = "VS_AVE_GCP_SERVICE_ACCOUNT_PRIVATE_KEY";

    /// <summary>
    ///     Google API Production Endpoint
    /// </summary>
    public static readonly Uri ProductionEndpointBaseUri = new("https://addressvalidation.googleapis.com");

    /// <summary>
    ///     Google Project ID
    /// </summary>
    public const string ProjectIdConfigurationKey = "VS_AVE_GCP_PROJECT_ID";

    /// <summary>
    ///     Service Account Email Address
    /// </summary>
    public const string ServiceAccountEmailConfigurationKey = "VS_AVE_GCP_SERVICE_ACCOUNT_EMAIL";

    /// <summary>
    ///     Countries that are supported by the Google Address Validation API.
    /// </summary>
    public static readonly IReadOnlySet<CountryCode> SupportedCountries = new HashSet<CountryCode>
    {
        CountryCode.AR,
        CountryCode.AT,
        CountryCode.AU,
        CountryCode.BE,
        CountryCode.BG,
        CountryCode.BR,
        CountryCode.CA,
        CountryCode.CH,
        CountryCode.CL,
        CountryCode.CO,
        CountryCode.CZ,
        CountryCode.DE,
        CountryCode.DK,
        CountryCode.EE,
        CountryCode.ES,
        CountryCode.FI,
        CountryCode.FR,
        CountryCode.GB,
        CountryCode.HR,
        CountryCode.HU,
        CountryCode.IE,
        CountryCode.IT,
        CountryCode.LT,
        CountryCode.LU,
        CountryCode.LV,
        CountryCode.MX,
        CountryCode.MY,
        CountryCode.NL,
        CountryCode.NO,
        CountryCode.NZ,
        CountryCode.PL,
        CountryCode.PR,
        CountryCode.PT,
        CountryCode.SE,
        CountryCode.SG,
        CountryCode.SI,
        CountryCode.SK,
        CountryCode.US
    };
}
