namespace Visus.AddressValidation.Integration.Google.Validation;

using System.Collections.Frozen;
using AddressValidation.Abstractions;
using AddressValidation.Validation;
using Model;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<GoogleAddressValidationRequest>
{
    /// <inheritdoc />
    protected override string ProviderName => "Google";

    /// <inheritdoc />
    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;
}
