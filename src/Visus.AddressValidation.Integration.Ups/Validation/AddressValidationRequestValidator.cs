namespace Visus.AddressValidation.Integration.Ups.Validation;

using System.Collections.Frozen;
using AddressValidation.Abstractions;
using AddressValidation.Validation;
using Configuration;
using Microsoft.Extensions.Options;
using Model;
using Resources;

internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<UpsAddressValidationRequest>
{
    private readonly IOptions<UpsServiceOptions> _options;

    private readonly HashSet<string> _supportedDevelopmentRegions = new(StringComparer.OrdinalIgnoreCase)
    {
        "CA",
        "NY",
    };

    public AddressValidationRequestValidator(IOptions<UpsServiceOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    protected override string ProviderName => "UPS";

    /// <inheritdoc />
    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;

    protected override async ValueTask<bool> PreValidateAsync(UpsAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( !await base.PreValidateAsync(instance, results, cancellationToken).ConfigureAwait(false) )
        {
            return false;
        }

        if ( _options.Value.ClientEnvironment != ClientEnvironment.DEVELOPMENT )
        {
            return true;
        }

        if ( instance.Country.HasValue && instance.Country.Value != CountryCode.US )
        {
            results.Add(ValidationState.CreateError(Resources.Validation_Provider_OnlyValueSupportedInMode,
                nameof(instance.Country),
                CountryCode.US,
                "UPS",
                ClientEnvironment.DEVELOPMENT));

            return false;
        }

        if ( instance.Country is not CountryCode.US
          || string.IsNullOrWhiteSpace(instance.StateOrProvince)
          || _supportedDevelopmentRegions.Contains(instance.StateOrProvince) )
        {
            return true;
        }

        results.Add(ValidationState.CreateError(Resources.Validation_Provider_OnlyValuesSupportedInMode,
            nameof(instance.StateOrProvince),
            string.Join(", ", _supportedDevelopmentRegions),
            "UPS",
            ClientEnvironment.DEVELOPMENT));

        return false;
    }
}
