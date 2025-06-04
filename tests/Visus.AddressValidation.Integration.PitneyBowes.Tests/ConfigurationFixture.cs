namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

using Microsoft.Extensions.Configuration;

public sealed class ConfigurationFixture
{
    public IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                  .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                                                   {
                                                       new(Constants.DeveloperIdConfigurationKey, "00000000"),
                                                       new(Constants.ApiKeyConfigurationKey, "fyLfAoc11kAHXEAKw6nBzZOqslTyztjO"),
                                                       new(Constants.ApiSecretConfigurationKey, "IXbuIKj1SV7SaIzn")
                                                   })
                                                  .Build(); // remark: these values are fake
}
