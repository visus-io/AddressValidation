namespace Visus.AddressValidation.Ups.Extensions;

using AddressValidation.Abstractions;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up UPS Address Validation services in an <see cref="IServiceCollection" />.
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

		return services;
	}
}
