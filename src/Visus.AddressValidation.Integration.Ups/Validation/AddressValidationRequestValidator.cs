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
        "NY"
    };

    protected override ValueTask ValidateAsync(UpsAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( !Constants.SupportedCountries.Contains(instance.Country!.Value) )
        {
            results.Add(ValidationState.CreateError("{0}: {1} is currently not supported by the UPS Address Validation API.",
                                                    nameof(instance.Country),
                                                    instance.Country));
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
            results.Add(ValidationState.CreateError("{0}: Only the value {1} is supported by the UPS Address Validation API while in {2} mode.",
                                                    nameof(instance.Country),
                                                    CountryCode.US,
                                                    ClientEnvironment.DEVELOPMENT));
        }

        if ( instance.Country.Value == CountryCode.US
          && !string.IsNullOrWhiteSpace(instance.StateOrProvince)
          && !_supportedDevelopmentRegions.Contains(instance.StateOrProvince) )
        {
            results.Add(ValidationState.CreateError("{0}: Only the values {1} are supported by the UPS Address Validation API while in {2} mode.",
                                                    nameof(instance.StateOrProvince),
                                                    string.Join(", ", _supportedDevelopmentRegions),
                                                    ClientEnvironment.DEVELOPMENT));
        }

        return ValueTask.CompletedTask;
    }
}
