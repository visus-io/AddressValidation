namespace Visus.AddressValidation.Http;

/// <summary>
///     Provides custom response data from an API response type.
/// </summary>
public interface ICustomResponseData
{
    /// <summary>
    ///     Gets the custom response data as a dictionary of key-value pairs.
    /// </summary>
    /// <returns>A read-only dictionary containing the custom response data.</returns>
    IReadOnlyDictionary<string, object?> GetCustomResponseData();
}
