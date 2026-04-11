namespace Visus.AddressValidation.Integration.Google;

using System.Collections.Frozen;
using AddressValidation.Abstractions;

/// <summary>
///     Configuration Constants for Google Service Integration
/// </summary>
public static class Constants
{
    /// <summary>
    ///     Countries that are supported by the Google Address Validation API.
    /// </summary>
    public static readonly FrozenSet<CountryCode> SupportedCountries = new HashSet<CountryCode>
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
        CountryCode.US,
    }.ToFrozenSet();
}
