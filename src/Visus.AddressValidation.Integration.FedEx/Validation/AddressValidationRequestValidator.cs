namespace Visus.AddressValidation.Integration.FedEx.Validation;

using AddressValidation.Abstractions;
using AddressValidation.Validation;
using Models;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<FedExAddressValidationRequest>
{
    /// <inheritdoc />
    protected override string ProviderName => "FedEx";

    /// <inheritdoc />
    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;
}
