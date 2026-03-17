namespace Visus.AddressValidation.Integration.Ups.Validation;

using AddressValidation.Abstractions;
using AddressValidation.Validation;
using Http;
using Microsoft.Extensions.Configuration;

internal sealed class AddressValidationRequestValidator(IConfiguration configuration)
    : AbstractAddressValidationRequestValidator<UpsAddressValidationRequest>
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    private readonly HashSet<string> _supportedDevelopmentRegions = new(StringComparer.OrdinalIgnoreCase)
    {
        "CA",
        "NY",
    };

    protected override ValueTask ValidateAsync(UpsAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( !Constants.SupportedCountries.Contains(instance.Country!.Value) )
        {
            results.Add(ValidationState.CreateError(ValidationMessages.CountryNotSupportedByProvider,
                nameof(instance.Country),
                instance.Country,
                "UPS"));
        }

        if ( !Enum.TryParse(_configuration[Constants.ClientEnvironmentConfigurationKey], out ClientEnvironment clientEnvironment) )
        {
            clientEnvironment = ClientEnvironment.DEVELOPMENT;
        }

        if ( clientEnvironment != ClientEnvironment.DEVELOPMENT )
        {
            return ValueTask.CompletedTask;
        }

        if ( instance.Country.Value != CountryCode.US )
        {
            results.Add(ValidationState.CreateError(ValidationMessages.OnlyValueSupportedInDevelopmentMode,
                nameof(instance.Country),
                CountryCode.US,
                "UPS",
                ClientEnvironment.DEVELOPMENT));
        }

        if ( instance.Country.Value == CountryCode.US
          && !string.IsNullOrWhiteSpace(instance.StateOrProvince)
          && !_supportedDevelopmentRegions.Contains(instance.StateOrProvince) )
        {
            results.Add(ValidationState.CreateError(ValidationMessages.OnlyValuesSupportedInDevelopmentMode,
                nameof(instance.StateOrProvince),
                string.Join(", ", _supportedDevelopmentRegions),
                "UPS",
                ClientEnvironment.DEVELOPMENT));
        }

        return base.ValidateAsync(instance, results, cancellationToken);
    }
}
