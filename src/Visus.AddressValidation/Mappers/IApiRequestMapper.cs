namespace Visus.AddressValidation.Mappers;

using Models;

/// <summary>
///     Defines a mapper that converts an address validation request into a
///     provider-specific API request.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the address validation request, which must derive from
///     <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiRequest">
///     The type of the provider-specific API request object.
/// </typeparam>
public interface IApiRequestMapper<in TRequest, out TApiRequest>
    where TRequest : AbstractAddressValidationRequest
    where TApiRequest : class
{
    /// <summary>
    ///     Maps the specified address validation request to a provider-specific
    ///     API request object.
    /// </summary>
    /// <param name="request">
    ///     The address validation request to map.
    /// </param>
    /// <returns>
    ///     A provider-specific API request object populated from
    ///     <paramref name="request" />.
    /// </returns>
    TApiRequest Map(TRequest request);
}
