namespace Visus.AddressValidation.Validation;

/// <summary>
///     Represents the result returned by a validator.
/// </summary>
public interface IValidationResult
{
	/// <summary>
	///     Gets a set of validation errors.
	/// </summary>
	IReadOnlySet<ValidationState> Errors { get; }

	/// <summary>
	///     Gets an indicator that errors exist within the result.
	/// </summary>
	bool HasErrors { get; }

	/// <summary>
	///     Gets an indicator that warnings exist within the result.
	/// </summary>
	bool HasWarnings { get; }

	/// <summary>
	///     Gets a set of validation warnings.
	/// </summary>
	IReadOnlySet<ValidationState> Warnings { get; }
}
