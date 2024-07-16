namespace Visus.AddressValidation.Http.Abstractions;

using AddressValidation.Abstractions;
using FluentValidation.Results;

/// <summary>
///     Abstraction for implementing a translation between the api response and an instance that implements
///     <see cref="IAddressValidationResponse" />.
/// </summary>
public interface IApiAddressValidationResponse
{
	/// <summary>
	///     Converts the underlying service api response to an instance that implements
	///     <see cref="IAddressValidationResponse" />.
	/// </summary>
	/// <param name="validationResult">
	///     Current validation state (if any) of the response represented as an instance of
	///     <see cref="ValidationResult" />.
	/// </param>
	/// <returns>An instance that implements <see cref="IAddressValidationResponse" />.</returns>
	public IAddressValidationResponse ToAddressValidationResponse(ValidationResult? validationResult);
}
