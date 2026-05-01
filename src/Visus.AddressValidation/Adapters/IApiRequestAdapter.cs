namespace Visus.AddressValidation.Adapters;

using Models;

/// <summary>
///     Defines an adapter that translates an address validation request into an
///     API-specific request and returns the corresponding API response.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the address validation request, which must derive from
///     <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiResponse">
///     The type of the API response returned by the underlying service.
/// </typeparam>
public interface IApiRequestAdapter<in TRequest, TApiResponse>
    where TRequest : AbstractAddressValidationRequest
    where TApiResponse : class
{
    /// <summary>
    ///     Executes the address validation request against the underlying API and
    ///     returns the raw API response.
    /// </summary>
    /// <param name="request">The address validation request to execute.</param>
    /// <param name="cancellationToken">
    ///     A token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result
    ///     contains the API response, or <see langword="null" /> if no response
    ///     was returned.
    /// </returns>
    Task<TApiResponse?> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}
