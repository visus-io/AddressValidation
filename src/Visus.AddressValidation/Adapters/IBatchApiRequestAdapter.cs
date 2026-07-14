namespace Visus.AddressValidation.Adapters;

using Models;

/// <summary>
///     Defines an adapter that translates a batch of address validation requests into a single API-specific
///     request and returns the corresponding API response.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the address validation request, which must derive from <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiResponse">
///     The type of the API response returned by the underlying service.
/// </typeparam>
public interface IBatchApiRequestAdapter<in TRequest, TApiResponse>
    where TRequest : AbstractAddressValidationRequest
    where TApiResponse : class
{
    /// <summary>
    ///     Executes the specified <paramref name="requests" /> against the underlying API as a single batch call and
    ///     returns the raw API response.
    /// </summary>
    /// <param name="requests">The address validation requests to execute, in the order results should be returned in.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <remarks>
    ///     Implementations must submit <paramref name="requests" /> to the underlying API preserving the given
    ///     order — callers correlate the returned <typeparamref name="TApiResponse" /> back to individual requests
    ///     strictly by position.
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the API response, or
    ///     <see langword="null" /> if no response was returned.
    /// </returns>
    Task<TApiResponse?> ExecuteAsync(IReadOnlyList<TRequest> requests, CancellationToken cancellationToken);
}
