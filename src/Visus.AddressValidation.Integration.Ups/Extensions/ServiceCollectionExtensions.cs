namespace Visus.AddressValidation.Integration.Ups.Extensions;

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
///     Extension methods for setting up UPS Address Validation services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds <see cref="IAddressValidationService{TRequest}" /> and related services to the
    ///     <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration" /> used to bind <see cref="UpsServiceOptions" />.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddUpsAddressValidation(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<UpsServiceOptions>()
                .Bind(configuration.GetSection(UpsServiceOptions.SectionName))
                .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<UpsServiceOptions>, UpsServiceOptionsValidator>();

        services.TryAddSingleton<UpsAuthenticationService>();

        services.TryAddScoped<IApiResponseMapper<ApiResponse>, AddressValidationResponseMapper>();
        services.TryAddScoped<IApiRequestMapper<UpsAddressValidationRequest, ApiRequest>, AddressValidationRequestMapper>();

        services.TryAddScoped<IValidator<UpsAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();
        services.TryAddScoped<IApiRequestAdapter<UpsAddressValidationRequest, ApiResponse>, ApiRequestAdapter>();

        services.TryAddScoped<IAddressValidationService<UpsAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<UpsAuthenticationClient>()
                .AddAuthenticationClientResilienceHandler();

        services.AddHttpClient<UpsAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization",])
                .AddHttpMessageHandler(provider =>
                 {
                     UpsAuthenticationService authenticationService = provider.GetRequiredService<UpsAuthenticationService>();
                     return new BearerTokenDelegatingHandler<UpsAuthenticationClient>(authenticationService);
                 })
                .AddAddressValidationClientResilienceHandler();

        return services;
    }
}
