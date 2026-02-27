namespace Visus.AddressValidation.Integration.Ups.Extensions;

using AddressValidation.Http;
using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services;
using Validation;

/// <summary>
///     Extension methods for setting up UPS Address Validation services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds <see cref="IAddressValidationService{TRequest}" /> and related services to the
    ///     <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddUpsAddressValidation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<UpsAuthenticationService>();

        services.TryAddScoped<IValidator<UpsAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();

        services.TryAddScoped<IAddressValidationService<UpsAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<UpsAuthenticationClient>()
                .AddStandardResilienceHandler();

        services.AddHttpClient<UpsAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization",])
                .AddHttpMessageHandler(provider =>
                 {
                     UpsAuthenticationService authenticationService = provider.GetRequiredService<UpsAuthenticationService>();
                     return new BearerTokenDelegatingHandler<UpsAuthenticationClient>(authenticationService);
                 })
                .AddStandardResilienceHandler();

        return services;
    }
}
