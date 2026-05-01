namespace Visus.AddressValidation.Mappers;

using Models;
using Validation;

/// <summary>
///     Abstraction for mapping an API response to an <see cref="IAddressValidationResponse" />.
/// </summary>
/// <typeparam name="TResponse">
///     The type of the underlying API response.
/// </typeparam>
public interface IApiResponseMapper<in TResponse>
    where TResponse : class
{
    /// <summary>
    ///     Maps <paramref name="response" /> to an instance that implements
    ///     <see cref="IAddressValidationResponse" />.
    /// </summary>
    /// <param name="response">The underlying API response returned by the address validation service.</param>
    /// <param name="validationResult">
    ///     Current validation state (if any) of the response represented as an instance of
    ///     <see cref="IValidationResult" />.
    /// </param>
    /// <returns>An instance that implements <see cref="IAddressValidationResponse" />.</returns>
    IAddressValidationResponse Map(TResponse response, IValidationResult? validationResult = null);
}
