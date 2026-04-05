namespace Visus.AddressValidation.Integration.PitneyBowes.Validation;

using System.Collections.Frozen;
using AddressValidation.Abstractions;
using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<PitneyBowesAddressValidationRequest>
{
    /// <inheritdoc />
    protected override string ProviderName => "Pitney Bowes";

    /// <inheritdoc />
    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;
}
