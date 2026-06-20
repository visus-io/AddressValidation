namespace Visus.AddressValidation.Integration.PitneyBowes.Validation;

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using AddressValidation.Abstractions;
using AddressValidation.Validation;
using Model;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<PitneyBowesAddressValidationRequest>
{
    /// <inheritdoc />
    protected override string ProviderName => "Pitney Bowes";

    /// <inheritdoc />
    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;
}
