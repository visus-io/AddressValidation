namespace Visus.AddressValidation.Integration.PitneyBowes.Validation;

using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<PitneyBowesAddressValidationRequest>
{
    protected override ValueTask ValidateAsync(PitneyBowesAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( !Constants.SupportedCountries.Contains(instance.Country!.Value) )
        {
            results.Add(ValidationState.CreateError("{0}: {1} is currently not supported by the Pitney Bowes Address Validation API.", nameof(instance.Country), instance.Country));
        }

        return base.ValidateAsync(instance, results, cancellationToken);
    }
}
