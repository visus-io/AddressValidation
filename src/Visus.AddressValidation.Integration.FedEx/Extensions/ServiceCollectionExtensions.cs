namespace Visus.AddressValidation.Integration.FedEx.Extensions;

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
using Models;
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

        services.AddOptions<FedExServiceOptions>()
                .BindConfiguration(FedExServiceOptions.SectionName)
                .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<FedExServiceOptions>, FedExServiceOptionsValidator>();

        services.TryAddSingleton<FedExAuthenticationService>();

        services.TryAddScoped<IApiResponseMapper<ApiResponse>, AddressValidationResponseMapper>();
        services.TryAddScoped<IApiRequestMapper<FedExAddressValidationRequest, ApiRequest>, AddressValidationRequestMapper>();
        services.TryAddScoped<IApiRequestAdapter<FedExAddressValidationRequest, ApiResponse>, ApiRequestAdapter>();

        services.TryAddScoped<IValidator<FedExAddressValidationRequest>, AddressValidationRequestValidator>();
        services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();

        services.TryAddScoped<IAddressValidationService<FedExAddressValidationRequest>, AddressValidationService>();

        services.AddHttpClient<FedExAuthenticationClient>()
                .AddAuthenticationClientResilienceHandler();

        services.AddHttpClient<FedExAddressValidationClient>()
                .RedactLoggedHeaders(["Authorization",])
                .AddHttpMessageHandler(provider =>
                 {
                     FedExAuthenticationService authenticationService =
                         provider.GetRequiredService<FedExAuthenticationService>();

                     return new BearerTokenDelegatingHandler<FedExAuthenticationClient>(authenticationService);
                 })
                .AddAddressValidationClientResilienceHandler();

        return services;
    }
}
