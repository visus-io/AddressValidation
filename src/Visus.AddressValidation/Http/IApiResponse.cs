namespace Visus.AddressValidation.Http;

using Model;
using Validation;

/// <summary>
///     Abstraction for implementing processing of results returned by an address validation API.
/// </summary>
public interface IApiResponse
{
    /// <summary>
    ///     Converts the underlying service api response to an instance that implements
    ///     <see cref="IAddressValidationResponse" />.
    /// </summary>
    /// <param name="validationResult">
    ///     Current validation state (if any) of the response represented as an instance of
    ///     <see cref="IValidationResult" />.
    /// </param>
    /// <returns>An instance that implements <see cref="IAddressValidationResponse" />.</returns>
    IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null);
}
