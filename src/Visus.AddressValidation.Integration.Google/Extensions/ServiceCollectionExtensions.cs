namespace Visus.AddressValidation.Integration.Google.Extensions;

using AddressValidation.Http;
using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
	/// <returns>The same service collection so that multiple calls can be chained.</returns>
	public static IServiceCollection AddGoogleAddressValidation(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		services.TryAddSingleton<GoogleAuthenticationService>();

		services.TryAddScoped<IValidator<GoogleAddressValidationRequest>, AddressValidationRequestValidator>();
		services.TryAddScoped<IValidator<ApiResponse>, ApiResponseValidator>();

		services.TryAddScoped<IAddressValidationService<GoogleAddressValidationRequest>, AddressValidationService>();

		services.AddHttpClient<GoogleAuthenticationClient>()
				.AddStandardResilienceHandler();

		services.AddHttpClient<GoogleAddressValidationClient>()
				.RedactLoggedHeaders(["Authorization", "X-Goog-User-Project"])
				.AddHttpMessageHandler(provider =>
									   {
										   GoogleAuthenticationService authenticationService = provider.GetRequiredService<GoogleAuthenticationService>();
										   return new BearerTokenDelegatingHandler<GoogleAuthenticationClient>(authenticationService);
									   })
				.AddStandardResilienceHandler();


		return services;
	}
}
