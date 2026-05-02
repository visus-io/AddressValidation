namespace Visus.AddressValidation.Integration.PitneyBowes.Extensions;

using Adapters;
using AddressValidation.Adapters;
using AddressValidation.Extensions;
using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Clients;
using Configuration;
using Contracts;
using Http;
using Mappers;
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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddPitneyBowesAddressValidation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<PitneyBowesServiceOptions>()
                .BindConfiguration(PitneyBowesServiceOptions.SectionName)
                .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<PitneyBowesServiceOptions>, PitneyBowesServiceOptionsValidator>();

        services.TryAddSingleton<PitneyBowesAuthenticationService>();

        services.TryAddScoped<IApiResponseMapper<ApiResponse>, AddressValidationResponseMapper>();
        services.TryAddScoped<IApiRequestMapper<PitneyBowesAddressValidationRequest, ApiRequest>, AddressValidationRequestMapper>();

        services.TryAddScoped<IValidator<PitneyBowesAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();
        services.TryAddScoped<IApiRequestAdapter<PitneyBowesAddressValidationRequest, ApiResponse>, ApiRequestAdapter>();

        services.TryAddScoped<IAddressValidationService<PitneyBowesAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<PitneyBowesAuthenticationClient>()
                .AddAuthenticationClientResilienceHandler();

        services.AddHttpClient<PitneyBowesAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization",])
                .AddHttpMessageHandler(provider =>
                 {
                     PitneyBowesAuthenticationService authenticationService = provider.GetRequiredService<PitneyBowesAuthenticationService>();
                     return new BearerTokenDelegatingHandler<PitneyBowesAuthenticationClient>(authenticationService);
                 })
                .AddAddressValidationClientResilienceHandler();

        return services;
    }
}
