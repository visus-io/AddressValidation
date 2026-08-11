namespace Visus.AddressValidation.Validation;

/// <summary>
///     Base class for implementing a validator.
/// </summary>
/// <typeparam name="T">The object instance to be validated.</typeparam>
public abstract class AbstractValidator<T> : IValidator<T>
    where T : class
{
    /// <inheritdoc />
    public ValueTask<IValidationResult> ExecuteAsync(T instance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);
        return ExecuteInternalAsync(new ValidationContext<T>(instance), cancellationToken);
    }

    /// <summary>
    ///     Determines if validation should continue. Override it to modify the instance or validation state before
    ///     execution.
    /// </summary>
    /// <param name="instance">The instance to perform validation against.</param>
    /// <param name="results">The set (collection) of <see cref="ValidationState" /> objects for the current instance.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns><see langword="true" /> to continue with validation; otherwise, <see langword="false" />.</returns>
    protected virtual ValueTask<bool> PreValidateAsync(T instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(true);
    }

    /// <summary>
    ///     Validates the specified instance.
    /// </summary>
    /// <param name="instance">The object to perform validation against.</param>
    /// <param name="results">The set (collection) of <see cref="ValidationState" /> objects for the current instance.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    protected virtual ValueTask ValidateAsync(T instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    private async ValueTask<IValidationResult> ExecuteInternalAsync(ValidationContext<T> context, CancellationToken cancellationToken)
    {
        if ( !await PreValidateAsync(context.Instance, context.ValidationResults, cancellationToken).ConfigureAwait(false) )
        {
            return new ValidationResult(context.ValidationResults);
        }

        await ValidateAsync(context.Instance, context.ValidationResults, cancellationToken).ConfigureAwait(false);

        return new ValidationResult(context.ValidationResults);
    }
}
