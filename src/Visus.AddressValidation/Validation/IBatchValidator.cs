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
    /// <param name="expectedResultCount">
    ///     The number of items expected in the result (the count of requests actually sent to the API).
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     A list of exactly <paramref name="expectedResultCount" /> <see cref="IValidationResult" /> objects,
    ///     positionally aligned with the items sent to the API.
    /// </returns>
    ValueTask<IReadOnlyList<IValidationResult>> ExecuteAsync(T instance, int expectedResultCount, CancellationToken cancellationToken = default);
}
