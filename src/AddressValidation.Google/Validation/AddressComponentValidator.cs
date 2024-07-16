namespace Visus.AddressValidation.Google.Validation;

using Abstractions;
using FluentValidation;
using Http;

internal sealed class AddressComponentValidator : AbstractValidator<ApiAddressValidationResponse.AddressComponent>
{
	private readonly HashSet<ConfirmationLevel> _tenuousConfirmations =
	[
		ConfirmationLevel.UNCONFIRMED_BUT_PLAUSIBLE,
		ConfirmationLevel.UNCONFIRMED_AND_SUSPICIOUS
	];

	public AddressComponentValidator()
	{
		RuleFor(r => r.ConfirmationLevel)
		   .NotEqual(ConfirmationLevel.CONFIRMATION_LEVEL_UNSPECIFIED);

		RuleFor(r => r.ConfirmationLevel)
		   .Must(m => !_tenuousConfirmations.Contains(m))
		   .WithSeverity(Severity.Warning)
		   .WithMessage("3"); // TODO: message about partial confirmation
	}
}
