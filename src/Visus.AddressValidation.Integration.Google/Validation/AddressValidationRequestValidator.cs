namespace Visus.AddressValidation.Integration.Google.Validation;

using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<GoogleAddressValidationRequest>
{
    protected override ValueTask ValidateAsync(GoogleAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( !Constants.SupportedCountries.Contains(instance.Country!.Value) )
        {
            results.Add(ValidationState.CreateError("{0}: {1} is currently not supported by the Google Address Validation API.", nameof(instance.Country), instance.Country));
        }

        return ValueTask.CompletedTask;
    }
}
