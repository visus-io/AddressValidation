namespace Visus.AddressValidation.Model;

using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

/// <summary>
///     Represents an empty validation response
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class EmptyAddressValidationResponse : AbstractAddressValidationResponse
{
	/// <inheritdoc />
	public EmptyAddressValidationResponse(ValidationResult? validationResult)
		: base(validationResult)
	{
	}
}
