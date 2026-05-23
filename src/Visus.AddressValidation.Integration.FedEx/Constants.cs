namespace Visus.AddressValidation.Integration.FedEx;

using System.Collections.Frozen;
using AddressValidation.Abstractions;

/// <summary>
///     Configuration constants for FedEx Service Integration
/// </summary>
public static class Constants
{
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
    public static readonly FrozenSet<CountryCode> SupportedCountries =
    [
        CountryCode.AR,
        CountryCode.AT,
        CountryCode.AU,
        CountryCode.AW,
        CountryCode.BB,
        CountryCode.BE,
        CountryCode.BM,
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
    ];

    /// <summary>
    ///     IETF BCP 47 locale tags that are supported by the FedEx Address Validation API.
    /// </summary>
    public static readonly FrozenSet<string> SupportedLocales =
    [
        "ar-SA",
        "bg-BG",
        "zh-CN",
        "zh-TW",
        "hr-HR",
        "cs-CZ",
        "da-DK",
        "nl-NL",
        "en-US",
        "et-EE",
        "fi-FI",
        "fr-FR",
        "de-DE",
        "el-GR",
        "he-IL",
        "hu-HU",
        "is-IS",
        "it-IT",
        "ja-JP",
        "ko-KR",
        "lv-LV",
        "lt-LT",
        "ms-MY",
        "nb-NO",
        "pl-PL",
        "pt-BR",
        "pt-PT",
        "ro-RO",
        "ru-RU",
        "sr-RS",
        "sk-SK",
        "sl-SI",
        "es-419",
        "es-ES",
        "sv-SE",
        "th-TH",
        "tr-TR",
        "uk-UA",
        "vi-VN",
    ];
}
