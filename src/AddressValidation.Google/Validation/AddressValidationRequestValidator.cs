namespace Visus.AddressValidation.Google.Validation;

using AddressValidation.Abstractions;
using FluentValidation;
using Http;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<GoogleAddressValidationRequest>
{
	private readonly HashSet<CountryCode> _supportedRegions =
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
	];

	public AddressValidationRequestValidator()
	{
		When(w => w.Country is not null,
			 () =>
			 {
				 RuleFor(r => r.Country)
					.Must(m => _supportedRegions.Contains(m!.Value))
					.WithMessage("The country '{PropertyValue}' is not supported by the Google Address Validation API.");
			 });
	}
}
