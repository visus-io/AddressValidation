namespace Visus.AddressValidation.Integration.FedEx.Validation;

using System.Collections.Frozen;
using AddressValidation.Abstractions;
using AddressValidation.Validation;
using Model;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<FedExAddressValidationRequest>
{
    /// <inheritdoc />
    protected override string ProviderName => "FedEx";

    /// <inheritdoc />
    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;
}
