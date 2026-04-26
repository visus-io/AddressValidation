namespace Visus.AddressValidation;

using System.Collections.Frozen;
using Abstractions;

internal static class Constants
{
    public static FrozenSet<CountryCode> CityStates =>
    [
        CountryCode.MC,
        CountryCode.SG,
        CountryCode.VA,
    ];

    public static FrozenSet<CountryCode> NoPostalCode =>
    [
        CountryCode.AE,
        CountryCode.AG,
        CountryCode.AO,
        CountryCode.AW,
        CountryCode.BF,
        CountryCode.BI,
        CountryCode.BJ,
        CountryCode.BQ,
        CountryCode.BS,
        CountryCode.BW,
        CountryCode.BZ,
        CountryCode.CD,
        CountryCode.CF,
        CountryCode.CG,
        CountryCode.CI,
        CountryCode.CK,
        CountryCode.CM,
        CountryCode.CW,
        CountryCode.DJ,
        CountryCode.DM,
        CountryCode.ER,
        CountryCode.FJ,
        CountryCode.GA,
        CountryCode.GD,
        CountryCode.GM,
        CountryCode.GN,
        CountryCode.GQ,
        CountryCode.GW,
        CountryCode.GY,
        CountryCode.HK,
        CountryCode.HM,
        CountryCode.KI,
        CountryCode.KN,
        CountryCode.ML,
        CountryCode.MO,
        CountryCode.MR,
        CountryCode.MW,
        CountryCode.NR,
        CountryCode.NU,
        CountryCode.QA,
        CountryCode.RW,
        CountryCode.SB,
        CountryCode.SC,
        CountryCode.SR,
        CountryCode.SS,
        CountryCode.ST,
        CountryCode.SX,
        CountryCode.TD,
        CountryCode.TF,
        CountryCode.TG,
        CountryCode.TK,
        CountryCode.TL,
        CountryCode.TO,
        CountryCode.TV,
        CountryCode.UG,
        CountryCode.VU,
        CountryCode.WS,
        CountryCode.YE,
        CountryCode.ZW,
        CountryCode.ZZ,
    ];
}
