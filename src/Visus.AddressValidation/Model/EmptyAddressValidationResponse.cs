namespace Visus.AddressValidation.Model;

using System.Diagnostics.CodeAnalysis;
using Validation;

/// <summary>
///     Represents an empty validation response
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="EmptyAddressValidationResponse" />.
/// </remarks>
/// <param name="validationResult">
///     Current validation state of the response represented as an instance of
///     <see cref="IValidationResult" />.
/// </param>
[ExcludeFromCodeCoverage]
public sealed class EmptyAddressValidationResponse(IValidationResult? validationResult = null)
	: AbstractAddressValidationResponse(validationResult);
