namespace Visus.AddressValidation.Mappers;

using Models;
using Validation;

/// <summary>
///     Abstraction for mapping a single item within a batch API response to an <see cref="IAddressValidationResponse" />.
/// </summary>
/// <typeparam name="TResponse">
///     The type of API response the provider returns.
/// </typeparam>
public interface IBatchApiResponseMapper<in TResponse>
    where TResponse : class
{
    /// <summary>
    ///     Maps the item at <paramref name="index" /> within <paramref name="response" /> to an instance that
    ///     implements <see cref="IAddressValidationResponse" />.
    /// </summary>
    /// <param name="response">The batch API response from the provider.</param>
    /// <param name="index">The position, within the batch, of the item to map.</param>
    /// <param name="validationResult">
    ///     The current <see cref="IValidationResult" />, or <see langword="null" /> if none exists.
    /// </param>
    /// <returns>An instance that implements <see cref="IAddressValidationResponse" />.</returns>
    IAddressValidationResponse Map(TResponse response, int index, IValidationResult? validationResult = null);
}
