namespace Visus.AddressValidation.Integration.PitneyBowes.Validation;

using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<PitneyBowesAddressValidationRequest>
{
    protected override ValueTask ValidateAsync(PitneyBowesAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( !Constants.SupportedCountries.Contains(instance.Country!.Value) )
        {
            results.Add(ValidationState.CreateError(ValidationMessages.CountryNotSupportedByProvider, nameof(instance.Country), instance.Country, "Pitney Bowes"));
        }

        return base.ValidateAsync(instance, results, cancellationToken);
    }
}
