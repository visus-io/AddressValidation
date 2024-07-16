namespace Visus.AddressValidation;

using FluentValidation;

/// <summary>
///     Base Validator for <see cref="AbstractAddressValidationRequest" /> instances.
/// </summary>
public abstract class AbstractAddressValidationRequestValidator<T> : AbstractValidator<T>
	where T : AbstractAddressValidationRequest
{
	/// <summary>
	///     Initializes a new instance of <see cref="AbstractAddressValidationRequestValidator{T}" />.
	/// </summary>
	/// <remarks>Contains conditional validation for countries that do not support a state/province.</remarks>
	protected AbstractAddressValidationRequestValidator()
	{
		RuleFor(r => r.Country).NotNull();

		When(w => w.Country is not null,
			 () =>
			 {
				 RuleFor(r => r.AddressLines).NotEmpty();

				 RuleFor(r => r.CityOrTown)
					.NotEmpty()
					.When(w => !AddressGlobals.CityStates.Contains(w.Country!.Value));

				 RuleFor(r => r.StateOrProvince)
					.NotEmpty()
					.When(w => !AddressGlobals.CityStates.Contains(w.Country!.Value));

				 RuleFor(r => r.PostalCode)
					.NotEmpty()
					.When(w => !AddressGlobals.NoPostalCode.Contains(w.Country!.Value));

				 RuleFor(r => r.Country)
					.Must(m => !AddressGlobals.NoPostalCode.Contains(m!.Value))
					.WithMessage("The country '{PropertyValue}' is currently not supported for address validation.");

				 RuleForEach(r => r.AddressLines).NotEmpty();
			 });
	}
}
