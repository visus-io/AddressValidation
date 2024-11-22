namespace Visus.AddressValidation;

using Abstractions;

internal static class Constants
{
	public static IReadOnlySet<CountryCode> CityStates => new HashSet<CountryCode>
	{
		CountryCode.MC,
		CountryCode.SG,
		CountryCode.VA
	};

	public static IReadOnlySet<CountryCode> NoPostalCode => new HashSet<CountryCode>
	{
		CountryCode.AE,
		CountryCode.AG,
		CountryCode.AO,
		CountryCode.AW,
		CountryCode.BF,
		CountryCode.BI,
		CountryCode.BJ,
		CountryCode.BO,
		CountryCode.BQ,
		CountryCode.BS,
		CountryCode.BW,
		CountryCode.BZ,
		CountryCode.CD,
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
		CountryCode.GH,
		CountryCode.GM,
		CountryCode.GQ,
		CountryCode.GY,
		CountryCode.HK,
		CountryCode.HM,
		CountryCode.KI,
		CountryCode.KN,
		CountryCode.KP,
		CountryCode.LY,
		CountryCode.ML,
		CountryCode.MO,
		CountryCode.MR,
		CountryCode.MW,
		CountryCode.NU,
		CountryCode.QA,
		CountryCode.RW,
		CountryCode.SB,
		CountryCode.SC,
		CountryCode.SH,
		CountryCode.SR,
		CountryCode.ST,
		CountryCode.SX,
		CountryCode.SY,
		CountryCode.TF,
		CountryCode.TG,
		CountryCode.TK,
		CountryCode.TL,
		CountryCode.TO,
		CountryCode.TV,
		CountryCode.UG,
		CountryCode.VU,
		CountryCode.YE,
		CountryCode.ZW,
		CountryCode.ZZ
	};
}
