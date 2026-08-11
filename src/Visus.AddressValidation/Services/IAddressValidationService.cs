namespace Visus.AddressValidation.Services;

using Models;

/// <summary>
///     Abstraction for implementing an address validation service.
/// </summary>
/// <typeparam name="TRequest">
///     The provider-specific request type. It must derive from
///     <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
public interface IAddressValidationService<in TRequest>
    where TRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Validates the <paramref name="request" /> instance asynchronously.
    /// </summary>
    /// <param name="request">The <typeparamref name="TRequest" /> instance to validate.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <remarks>
    ///     The returned <see cref="IAddressValidationResponse" /> may be <see langword="null" /> if the provider
    ///     returns no response. If the request or response fails validation, this method returns an
    ///     <see cref="EmptyAddressValidationResponse" /> instead.
    /// </remarks>
    /// <returns>The validation result as an <see cref="IAddressValidationResponse" />.</returns>
    Task<IAddressValidationResponse?> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
