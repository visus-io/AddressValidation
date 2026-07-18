namespace Visus.AddressValidation.Mappers;

using Models;
using Validation;

/// <summary>
///     Abstraction for mapping a single item within a batch API response to an <see cref="IAddressValidationResponse" />.
/// </summary>
/// <typeparam name="TResponse">
///     The type of the underlying API response.
/// </typeparam>
public interface IBatchApiResponseMapper<in TResponse>
    where TResponse : class
{
    /// <summary>
    ///     Maps the item at <paramref name="index" /> within <paramref name="response" /> to an instance that
    ///     implements <see cref="IAddressValidationResponse" />.
    /// </summary>
    /// <param name="response">The underlying batch API response returned by the address validation service.</param>
    /// <param name="index">The position, within the batch, of the item to map.</param>
    /// <param name="validationResult">
    ///     Current validation state (if any) of the item represented as an instance of <see cref="IValidationResult" />.
    /// </param>
    /// <returns>An instance that implements <see cref="IAddressValidationResponse" />.</returns>
    IAddressValidationResponse Map(TResponse response, int index, IValidationResult? validationResult = null);
}
