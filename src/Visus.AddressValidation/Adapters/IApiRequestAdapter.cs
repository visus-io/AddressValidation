namespace Visus.AddressValidation.Adapters;

using Models;

/// <summary>
///     Defines an adapter that translates an address validation request into an
///     API-specific request. It sends the request to the provider and returns
///     the API response.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the address validation request, which must derive from
///     <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiResponse">
///     The type of API response the provider returns.
/// </typeparam>
public interface IApiRequestAdapter<in TRequest, TApiResponse>
    where TRequest : AbstractAddressValidationRequest
    where TApiResponse : class
{
    /// <summary>
    ///     Executes the address validation request against the provider and
    ///     returns the API response.
    /// </summary>
    /// <param name="request">The address validation request to execute.</param>
    /// <param name="cancellationToken">
    ///     A token that cancels the operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result
    ///     holds the API response, or <see langword="null" /> if the provider
    ///     returns none.
    /// </returns>
    Task<TApiResponse?> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}
