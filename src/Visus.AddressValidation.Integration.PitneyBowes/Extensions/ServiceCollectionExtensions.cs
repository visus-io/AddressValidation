namespace Visus.AddressValidation.Integration.PitneyBowes.Extensions;

using AddressValidation.Http;
using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services;
using Validation;

/// <summary>
///     Extension methods for setting up Pitney Bowes Address Validation serivces in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds <see cref="IAddressValidationService{TRequest}" /> and related services to the
    ///     <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddPitneyBowesAddressValidation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

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
