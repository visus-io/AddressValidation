namespace Visus.AddressValidation.Integration.PitneyBowes.Extensions;

using AddressValidation.Http;
using AddressValidation.Services;
using AddressValidation.Validation;
using Configuration;
using Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Model;
using Services;
using Validation;

/// <summary>
///     Extension methods for setting up Pitney Bowes Address Validation services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds <see cref="IAddressValidationService{TRequest}" /> and related services to the
    ///     <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration" /> used to bind <see cref="PitneyBowesServiceOptions" />.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddPitneyBowesAddressValidation(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<PitneyBowesServiceOptions>()
                .Bind(configuration.GetSection(nameof(PitneyBowesServiceOptions)))
                .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<PitneyBowesServiceOptions>, PitneyBowesServiceOptionsValidator>();

        services.TryAddSingleton<PitneyBowesAuthenticationService>();

        services.TryAddScoped<IValidator<PitneyBowesAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();

        services.TryAddScoped<IAddressValidationService<PitneyBowesAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<PitneyBowesAuthenticationClient>()
                .AddStandardResilienceHandler();

        services.AddHttpClient<PitneyBowesAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization",])
                .AddHttpMessageHandler(provider =>
                 {
                     PitneyBowesAuthenticationService authenticationService = provider.GetRequiredService<PitneyBowesAuthenticationService>();
                     return new BearerTokenDelegatingHandler<PitneyBowesAuthenticationClient>(authenticationService);
                 })
                .AddStandardResilienceHandler();

        return services;
    }
}
