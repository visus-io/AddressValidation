namespace Visus.AddressValidation.Integration.FedEx.Tests;

using AddressValidation.Services;
using AddressValidation.Validation;
using Extensions;
using Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class ServiceCollectionExtensionFacts
{
    [Fact]
    public void AddUpsAddressValidation_Default()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDistributedMemoryCache();
        serviceCollection.AddSingleton<IConfiguration>(_ => new ConfigurationBuilder().Build());
        serviceCollection.AddFedExAddressValidation();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        using ( var scope = serviceProvider.CreateScope() )
        {
            Assert.NotNull(scope.ServiceProvider.GetService<IValidator<FedExAddressValidationRequest>>());
            Assert.NotNull(scope.ServiceProvider.GetService<IAddressValidationService<FedExAddressValidationRequest>>());
        }

        Assert.NotNull(serviceProvider.GetService<FedExAuthenticationClient>());
    }
}
