namespace Visus.AddressValidation.Validation;

using System.Runtime.CompilerServices;

/// <summary>
///     Base class for implemented a validator.
/// </summary>
/// <typeparam name="T">The object instance to be validated.</typeparam>
public abstract class AbstractValidator<T> : IValidator<T>
    where T : class
{
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<IValidationResult> ExecuteAsync(T instance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);
        return ExecuteInternalAsync(new ValidationContext<T>(instance), cancellationToken);
    }

    /// <summary>
    ///     Determines if validation should continue as well as providing a means to modify the instance and validation state
    ///     prior to execution.
    /// </summary>
    /// <param name="instance">The instance to perform validation against.</param>
    /// <param name="results">The set (collection) of <see cref="ValidationState" /> objects for the current instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
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
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
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
