namespace Visus.AddressValidation.Ups.Validation;

using Abstractions;
using FluentValidation;
using Http;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<UpsAddressValidationRequest>
{
	private readonly HashSet<CountryCode> _supportedRegions =
	[
		CountryCode.US,
		CountryCode.PR
	];
	
	public AddressValidationRequestValidator()
	{
		When(w => w.Country is not null,
			 () =>
			 {
				 RuleFor(r => r.Country)
					.Must(m => _supportedRegions.Contains(m!.Value))
					.WithMessage("The country '{PropertyValue}' is not supported by the UPS Address Validation API.");
			 });
	}
}
