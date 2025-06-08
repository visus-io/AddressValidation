using Microsoft.Extensions.DependencyInjection;
using Visus.AddressValidation.Services;

namespace Visus.AddressValidation.Integration.FedEx.Extensions;

using AddressValidation.Http;
using AddressValidation.Validation;
using Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services;
using Validation;

/// <summary>
///     Extension methods for setting up FedEx Address Validation services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds <see cref="IAddressValidationService{TRequest}" /> and related services to the
    ///     <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddFedExAddressValidation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<FedExAuthenticationService>();
        
        services.TryAddScoped<IValidator<FedExAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();
        
        services.TryAddScoped<IAddressValidationService<FedExAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<FedExAuthenticationClient>()
                .AddStandardResilienceHandler();

        services.AddHttpClient<FedExAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization"])
                .AddHttpMessageHandler(provider =>
                                       {
                                           FedExAuthenticationService authenticationService =
                                               provider.GetRequiredService<FedExAuthenticationService>();
                                           
                                           return new BearerTokenDelegatingHandler<FedExAuthenticationClient>(authenticationService);
                                       })
                .AddStandardResilienceHandler();

        return services;
    }
}
