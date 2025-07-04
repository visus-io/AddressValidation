namespace Visus.AddressValidation.Integration.FedEx.Validation;

using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<FedExAddressValidationRequest>
{
    protected override ValueTask ValidateAsync(FedExAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( !Constants.SupportedCountries.Contains(instance.Country!.Value) )
        {
            results.Add(ValidationState.CreateError("{0}: {1} is currently not supported by the FedEx Address Validation API.", nameof(instance.Country), instance.Country));
        }

        return base.ValidateAsync(instance, results, cancellationToken);
    }
}
