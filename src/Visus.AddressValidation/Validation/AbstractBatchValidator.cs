namespace Visus.AddressValidation.Validation;

/// <summary>
///     Base class for implementing a batch validator that produces one independent <see cref="IValidationResult" />
///     per item in a batch response.
/// </summary>
/// <typeparam name="T">The object instance to be validated.</typeparam>
public abstract class AbstractBatchValidator<T> : IBatchValidator<T>
    where T : class
{
    /// <inheritdoc />
    public ValueTask<IReadOnlyList<IValidationResult>> ExecuteAsync(T instance, int expectedResultCount, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentOutOfRangeException.ThrowIfNegative(expectedResultCount);

        return ExecuteInternalAsync(instance, expectedResultCount, cancellationToken);
    }

    /// <summary>
    ///     Determines if validation should continue, providing a means to add shared (batch-wide) validation state
    ///     that applies to every item prior to per-item validation.
    /// </summary>
    /// <param name="instance">The instance to perform validation against.</param>
    /// <param name="expectedResultCount">The number of items expected in the result.</param>
    /// <param name="results">
    ///     One <see cref="ISet{T}" /> of <see cref="ValidationState" /> objects per expected item, indexed
    ///     positionally.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns><see langword="true" /> to continue with per-item validation; otherwise, <see langword="false" />.</returns>
    protected virtual ValueTask<bool> PreValidateAsync(T instance, int expectedResultCount, IReadOnlyList<ISet<ValidationState>> results, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(true);
    }

    /// <summary>
    ///     Validates the specified instance, populating per-item validation state.
    /// </summary>
    /// <param name="instance">The object to perform validation against.</param>
    /// <param name="results">
    ///     One <see cref="ISet{T}" /> of <see cref="ValidationState" /> objects per expected item, indexed
    ///     positionally.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    protected virtual ValueTask ValidateAsync(T instance, IReadOnlyList<ISet<ValidationState>> results, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    private static IReadOnlyList<IValidationResult> BuildResults(IReadOnlyList<ISet<ValidationState>> perItemResults)
    {
        return [.. perItemResults.Select(static states => (IValidationResult)new ValidationResult((IReadOnlySet<ValidationState>)states)),];
    }

    private async ValueTask<IReadOnlyList<IValidationResult>> ExecuteInternalAsync(T instance, int expectedResultCount, CancellationToken cancellationToken)
    {
        List<ISet<ValidationState>> perItemResults = new(expectedResultCount);
        for ( int i = 0; i < expectedResultCount; i++ )
        {
            perItemResults.Add(new HashSet<ValidationState>());
        }

        if ( !await PreValidateAsync(instance, expectedResultCount, perItemResults, cancellationToken).ConfigureAwait(false) )
        {
            return BuildResults(perItemResults);
        }

        await ValidateAsync(instance, perItemResults, cancellationToken).ConfigureAwait(false);

        return BuildResults(perItemResults);
    }
}
