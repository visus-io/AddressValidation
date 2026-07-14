namespace Visus.AddressValidation.Services;

using Models;

/// <summary>
///     Opt-in abstraction for an <see cref="IAddressValidationService{TRequest}" /> whose underlying provider API
///     natively supports validating multiple addresses in a single call.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the validation request. Must derive from <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
public interface IBatchAddressValidationService<in TRequest>
    where TRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Validates the specified <paramref name="requests" /> asynchronously as a single batch call.
    /// </summary>
    /// <param name="requests">The addresses to validate, in the order results should be returned in.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <remarks>
    ///     The returned list always has the same length and the same order as <paramref name="requests" />. An entry
    ///     is an <see cref="EmptyAddressValidationResponse" /> when the corresponding request failed local
    ///     validation, or when the provider could not resolve that specific address. An entry is
    ///     <see langword="null" /> only when the entire underlying API call for the batch produced no response at
    ///     all (mirroring the "no response" semantics of <see cref="IAddressValidationService{TRequest}.ValidateAsync" />);
    ///     this can only ever apply to positions that held a locally-valid request.
    /// </remarks>
    /// <returns>
    ///     A task producing a list of <see cref="IAddressValidationResponse" /> (or <see langword="null" />),
    ///     positionally aligned with <paramref name="requests" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="requests" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown synchronously when <paramref name="requests" /> contains more items than the provider's maximum
    ///     supported batch size.
    /// </exception>
    Task<IReadOnlyList<IAddressValidationResponse?>> ValidateManyAsync(IReadOnlyList<TRequest> requests, CancellationToken cancellationToken = default);
}
