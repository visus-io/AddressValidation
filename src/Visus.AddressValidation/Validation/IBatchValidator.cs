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
    /// <param name="instance">The instance to validate.</param>
    /// <param name="requestIndexes">
    ///     The original, caller-facing index of each request sent to the provider, in the order sent. Its count is
    ///     the number of items expected in the result. Use these values, instead of the positional loop index,
    ///     when a validation message must reference an item's original position. Local validation may filter out
    ///     some requests before the batch call, so the loop index does not match the original position.
    /// </param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>
    ///     A list of exactly <c>requestIndexes.Count</c> <see cref="IValidationResult" /> objects, positionally
    ///     aligned with the items sent to the provider.
    /// </returns>
    ValueTask<IReadOnlyList<IValidationResult>> ExecuteAsync(T instance, IReadOnlyList<int> requestIndexes, CancellationToken cancellationToken = default);
}
