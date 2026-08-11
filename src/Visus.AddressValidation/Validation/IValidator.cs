namespace Visus.AddressValidation.Validation;

/// <summary>
///     Defines a validator for a given type.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public interface IValidator<in T>
    where T : class
{
    /// <summary>
    ///     Executes validation against the specified instance.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>An <see cref="IValidationResult" /> object containing any validation errors or warnings.</returns>
    ValueTask<IValidationResult> ExecuteAsync(T instance, CancellationToken cancellationToken = default);
}
