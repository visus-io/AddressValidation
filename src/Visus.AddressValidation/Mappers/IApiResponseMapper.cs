namespace Visus.AddressValidation.Mappers;

using Models;
using Validation;

/// <summary>
///     Abstraction for mapping an API response to an <see cref="IAddressValidationResponse" />.
/// </summary>
/// <typeparam name="TResponse">
///     The type of API response the provider returns.
/// </typeparam>
public interface IApiResponseMapper<in TResponse>
    where TResponse : class
{
    /// <summary>
    ///     Maps <paramref name="response" /> to an instance that implements
    ///     <see cref="IAddressValidationResponse" />.
    /// </summary>
    /// <param name="response">The API response from the provider.</param>
    /// <param name="validationResult">
    ///     The current <see cref="IValidationResult" />, or <see langword="null" /> if none exists.
    /// </param>
    /// <returns>An instance that implements <see cref="IAddressValidationResponse" />.</returns>
    IAddressValidationResponse Map(TResponse response, IValidationResult? validationResult = null);
}
