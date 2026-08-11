namespace Visus.AddressValidation.Adapters;

using Models;

/// <summary>
///     Defines an adapter that translates a batch of address validation requests into a single API-specific
///     request. It sends the request to the provider and returns the API response.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the address validation request, which must derive from <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiResponse">
///     The type of API response the provider returns.
/// </typeparam>
public interface IBatchApiRequestAdapter<in TRequest, TApiResponse>
    where TRequest : AbstractAddressValidationRequest
    where TApiResponse : class
{
    /// <summary>
    ///     Sends the specified <paramref name="requests" /> to the provider as a single batch call and returns
    ///     the API response.
    /// </summary>
    /// <param name="requests">The address validation requests to execute, listed in the order the response must preserve.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <remarks>
    ///     Implementations must submit <paramref name="requests" /> to the provider in the given order. Callers
    ///     correlate each item within the resulting <typeparamref name="TApiResponse" /> back to its request,
    ///     strictly by position.
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result holds the API response, or
    ///     <see langword="null" /> if the provider returns none.
    /// </returns>
    Task<TApiResponse?> ExecuteAsync(IReadOnlyList<TRequest> requests, CancellationToken cancellationToken);
}
