namespace Visus.AddressValidation.Mappers;

using Models;

/// <summary>
///     Defines a mapper that converts a batch of address validation requests into a single provider-specific API
///     request.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the address validation request, which must derive from <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiRequest">
///     The type of the provider-specific API request object.
/// </typeparam>
public interface IBatchApiRequestMapper<in TRequest, out TApiRequest>
    where TRequest : AbstractAddressValidationRequest
    where TApiRequest : class
{
    /// <summary>
    ///     Maps the specified address validation requests to a single provider-specific API request object.
    /// </summary>
    /// <param name="requests">The address validation requests to map, in the order results should be returned in.</param>
    /// <returns>
    ///     A provider-specific API request object populated from <paramref name="requests" />.
    /// </returns>
    TApiRequest Map(IReadOnlyList<TRequest> requests);
}
