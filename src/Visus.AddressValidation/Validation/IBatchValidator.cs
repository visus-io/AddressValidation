namespace Visus.AddressValidation.Validation;

/// <summary>
///     Defines a validator that produces one independent <see cref="IValidationResult" /> per item in a batch
///     response.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public interface IBatchValidator<in T>
    where T : class
{
    /// <summary>
    ///     Executes validation against the specified instance, producing one result per expected item.
    /// </summary>
    /// <param name="instance">The instance which will be validated against.</param>
    /// <param name="requestIndexes">
    ///     The original, caller-facing index of each request actually sent to the API, in the same order the
    ///     requests were sent. Its count is the number of items expected in the result; implementations should use
    ///     the values themselves (rather than the positional loop index) when a validation message needs to
    ///     reference an item's position, since requests that failed local validation may have been filtered out
    ///     before the API call.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     A list of exactly <c>requestIndexes.Count</c> <see cref="IValidationResult" /> objects, positionally
    ///     aligned with the items sent to the API.
    /// </returns>
    ValueTask<IReadOnlyList<IValidationResult>> ExecuteAsync(T instance, IReadOnlyList<int> requestIndexes, CancellationToken cancellationToken = default);
}
