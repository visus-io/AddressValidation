namespace Visus.AddressValidation.Integration.Google.Extensions;

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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Models;
using Services;
using Validation;

/// <summary>
///     Extension methods for setting up Google Address Validation services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds <see cref="IAddressValidationService{TRequest}" /> and related services to the
    ///     <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration" /> used to bind <see cref="GoogleServiceOptions" />.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddGoogleAddressValidation(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<GoogleServiceOptions>()
                .Bind(configuration.GetSection(GoogleServiceOptions.SectionName))
                .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<GoogleServiceOptions>, GoogleServiceOptionsValidator>();

        services.TryAddSingleton<GoogleAuthenticationService>();

        services.TryAddScoped<IApiResponseMapper<ApiResponse>, AddressValidationResponseMapper>();
        services.TryAddScoped<IApiRequestMapper<GoogleAddressValidationRequest, ApiRequest>, AddressValidationRequestMapper>();
        services.TryAddScoped<IApiRequestAdapter<GoogleAddressValidationRequest, ApiResponse>, ApiRequestAdapter>();

        services.TryAddScoped<IValidator<GoogleAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();

        services.TryAddScoped<IAddressValidationService<GoogleAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<GoogleAuthenticationClient>()
                .AddAuthenticationClientResilienceHandler();

        services.AddHttpClient<GoogleAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization",])
                .AddHttpMessageHandler(provider =>
                 {
                     GoogleAuthenticationService authenticationService = provider.GetRequiredService<GoogleAuthenticationService>();
                     return new BearerTokenDelegatingHandler<GoogleAuthenticationClient>(authenticationService);
                 })
                .AddAddressValidationClientResilienceHandler();

        return services;
    }
}
