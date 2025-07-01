namespace Visus.AddressValidation.Integration.FedEx.Tests;

using AddressValidation.Abstractions;
using Microsoft.Extensions.Configuration;

public sealed class ConfigurationFixture
{
    public IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                  .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                                                   {
                                                       new(Constants.AccountNumberConfigurationKey, "111111111"),
                                                       new(Constants.ClientEnvironmentConfigurationKey, nameof(ClientEnvironment.PRODUCTION)),
                                                       new(Constants.ClientIdConfigurationKey, "cmbp0y1op00016837sup6icq8"),
                                                       new(Constants.ClientSecretConfigurationKey, "m97VXpZsiiGjTwk1EvH3vwytekUE5KdeES6zaRuZcgHU4ZAz3G3+nxKpJ5msQ4Ee\nRQBAvxaHhz5zyHgUdkowNw==")
                                                   })
                                                  .Build(); // remark: these values are fake
}
