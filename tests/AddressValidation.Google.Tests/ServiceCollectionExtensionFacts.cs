namespace Visus.AddressValidation.Google.Tests;

using AddressValidation.Abstractions;
using Extensions;
using FluentValidation;
using Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class ServiceCollectionExtensionFacts
{
	[Fact]
	public void AddGoogleAddressValidation_Default()
	{
		var serviceCollection = new ServiceCollection();

		serviceCollection.AddSingleton<IConfiguration>(_ => new ConfigurationBuilder().Build());
		serviceCollection.AddGoogleAddressValidation();

		var serviceProvider = serviceCollection.BuildServiceProvider();

		using ( var scope = serviceProvider.CreateScope() )
		{
			Assert.NotNull(scope.ServiceProvider.GetService<IValidator<GoogleAddressValidationRequest>>());
			Assert.NotNull(scope.ServiceProvider.GetService<IAddressValidationService<GoogleAddressValidationRequest>>());
		}

		Assert.NotNull(serviceProvider.GetService<GoogleAddressValidationClient>());
	}
}
