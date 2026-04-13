namespace Visus.AddressValidation.Integration.FedEx.Extensions;

using AddressValidation.Http;
using AddressValidation.Services;
using AddressValidation.Validation;
using Configuration;
using Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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
    /// <param name="configuration">The <see cref="IConfiguration" /> used to bind <see cref="FedExServiceOptions" />.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddFedExAddressValidation(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<FedExServiceOptions>()
                .BindConfiguration(FedExServiceOptions.SectionName)
                .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<FedExServiceOptions>, FedExServiceOptionsValidator>();

        services.TryAddSingleton<FedExAuthenticationService>();

        services.TryAddScoped<IValidator<FedExAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();

        services.TryAddScoped<IAddressValidationService<FedExAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<FedExAuthenticationClient>()
                .AddStandardResilienceHandler();

        services.AddHttpClient<FedExAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization",])
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
