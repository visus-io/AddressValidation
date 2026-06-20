namespace Visus.AddressValidation.Integration.Google.Validation;

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using AddressValidation.Abstractions;
using AddressValidation.Validation;
using Models;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<GoogleAddressValidationRequest>
{
    /// <inheritdoc />
    protected override string ProviderName => "Google";

    /// <inheritdoc />
    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;
}
