namespace Visus.AddressValidation.Services;

using Models;

/// <summary>
///     Opt-in abstraction for an <see cref="IAddressValidationService{TRequest}" /> whose provider API natively
///     supports batches of addresses in a single call.
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
    /// <param name="requests">The addresses to validate, listed in the order the response must preserve.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <remarks>
    ///     The returned list has the same length and order as <paramref name="requests" />. An entry is an
    ///     <see cref="EmptyAddressValidationResponse" /> when the corresponding request fails local validation, or
    ///     when the provider cannot resolve that address. An entry is <see langword="null" /> only when the batch
    ///     API call produces no response at all. This matches the "no response" semantics of
    ///     <see cref="IAddressValidationService{TRequest}.ValidateAsync" />. A <see langword="null" /> entry can
    ///     only occur at a position that held a locally-valid request.
    /// </remarks>
    /// <returns>
    ///     A task that returns a list of <see cref="IAddressValidationResponse" /> (or <see langword="null" />)
    ///     items, positionally aligned with <paramref name="requests" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="requests" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown synchronously when <paramref name="requests" /> contains more items than the provider's maximum
    ///     supported batch size.
    /// </exception>
    /// <exception cref="InvalidImplementationException">
    ///     Thrown when the registered batch response validator returns a different number of results than the
    ///     number of items sent to the provider.
    /// </exception>
    Task<IReadOnlyList<IAddressValidationResponse?>> ValidateManyAsync(IReadOnlyList<TRequest> requests, CancellationToken cancellationToken = default);
}
