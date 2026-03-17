namespace Visus.AddressValidation.Integration.FedEx.Validation;

using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<FedExAddressValidationRequest>
{
    protected override ValueTask ValidateAsync(FedExAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( !Constants.SupportedCountries.Contains(instance.Country!.Value) )
        {
            results.Add(ValidationState.CreateError(ValidationMessages.CountryNotSupportedByProvider, nameof(instance.Country), instance.Country, "FedEx"));
        }

        return base.ValidateAsync(instance, results, cancellationToken);
    }
}
