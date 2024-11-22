namespace Visus.AddressValidation.Integration.PitneyBowes.Tests;

using AddressValidation.Services;
using AddressValidation.Validation;
using Extensions;
using Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class ServiceCollectionExtensionFacts
{
	[Fact]
	public void AddPitneyBowesValidation_Default()
	{
		var serviceCollection = new ServiceCollection();

		serviceCollection.AddDistributedMemoryCache();
		serviceCollection.AddSingleton<IConfiguration>(_ => new ConfigurationBuilder().Build());
		serviceCollection.AddPitneyBowesAddressValidation();

		var serviceProvider = serviceCollection.BuildServiceProvider();

		using ( var scope = serviceProvider.CreateScope() )
		{
			Assert.NotNull(scope.ServiceProvider.GetService<IValidator<PitneyBowesAddressValidationRequest>>());
			Assert.NotNull(scope.ServiceProvider.GetService<IAddressValidationService<PitneyBowesAddressValidationRequest>>());
		}

		Assert.NotNull(serviceProvider.GetService<PitneyBowesAddressValidationClient>());
	}
}
