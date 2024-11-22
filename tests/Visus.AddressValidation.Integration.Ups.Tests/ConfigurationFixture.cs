namespace Visus.AddressValidation.Integration.Ups.Tests;

using AddressValidation.Abstractions;
using Microsoft.Extensions.Configuration;

public sealed class ConfigurationFixture
{
	public IConfiguration Configuration { get; } = new ConfigurationBuilder()
												  .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
												   {
													   new(Constants.AccountNumberConfigurationKey, "00000N"),
													   new(Constants.ClientEnvironmentConfigurationKey, ClientEnvironment.PRODUCTION.ToString()),
													   new(Constants.ClientIdConfigurationKey, "cm0v41yuu00017537p3fob8t4"),
													   new(Constants.ClientSecretConfigurationKey, "c7a1ea27658203d400c8c944a1d51239f34554ef8bef4b77d4012d18deda0bc8")
												   })
												  .Build(); // remark: these values are fake
}
