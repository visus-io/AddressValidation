namespace Visus.AddressValidation.Integration.Google;

using AddressValidation.Abstractions;

/// <summary>
///     Configuration constants for the Google integration.
/// </summary>
public static class Constants
{
    /// <summary>
    ///     The Google authentication API endpoint. Used for both production and development environments.
    /// </summary>
    public static readonly Uri ProductionAuthenticationUri = new("https://oauth2.googleapis.com/token");

    /// <summary>
    ///     The Google Address Validation API endpoint. Used for both production and development environments.
    /// </summary>
    public static readonly Uri ProductionEndpointUri = new("https://addressvalidation.googleapis.com");

    /// <summary>
    ///     The countries the Google Address Validation API supports.
    /// </summary>
    public static readonly FrozenSet<CountryCode> SupportedCountries =
    [
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
        CountryCode.IN,
        CountryCode.IT,
        CountryCode.JP,
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
        CountryCode.US,
    ];
}
