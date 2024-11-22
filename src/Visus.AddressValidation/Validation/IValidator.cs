namespace Visus.AddressValidation.Validation;

/// <summary>
///     Defines a validator for a given type.
/// </summary>
/// <typeparam name="T">Object that will be validated.</typeparam>
public interface IValidator<in T>
	where T : class
{
	/// <summary>
	///     Executes validation against the specified instance.
	/// </summary>
	/// <param name="instance">The instance which will be validated against.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
	/// <returns>An <see cref="IValidationResult" /> object containing any validation errors or warnings.</returns>
	ValueTask<IValidationResult> ExecuteAsync(T instance, CancellationToken cancellationToken = default);
}
