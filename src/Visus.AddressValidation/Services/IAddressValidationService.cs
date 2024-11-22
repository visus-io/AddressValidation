namespace Visus.AddressValidation.Services;

using Http;
using Model;

/// <summary>
///     Abstraction for implementing an address validation service.
/// </summary>
/// <typeparam name="TRequest">The request that will be used for validation.</typeparam>
public interface IAddressValidationService<in TRequest>
	where TRequest : AbstractAddressValidationRequest
{
	/// <summary>
	///     Validates the <paramref name="request" /> instance asynchronously.
	/// </summary>
	/// <param name="request">Request to be validated represented as an instance of <typeparamref name="TRequest" />.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
	/// <remarks>
	///     The instance <see cref="IAddressValidationResponse" /> may be <see langword="null" /> if the request or validation
	///     failed.
	/// </remarks>
	/// <returns>Result of the validation as represented by an instance of <see cref="IAddressValidationResponse" />.</returns>
	ValueTask<IAddressValidationResponse?> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
