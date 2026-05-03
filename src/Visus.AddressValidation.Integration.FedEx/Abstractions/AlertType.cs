namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

using System.Text.Json.Serialization;

/// <summary>
///     Represents the type of alert returned in a FedEx API response.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<AlertType>))]
internal enum AlertType
{
    /// <summary>
    ///     An error returned in the API response.
    /// </summary>
    ERROR,

    /// <summary>
    ///     An informational note that does not indicate a problem.
    /// </summary>
    NOTE,

    /// <summary>
    ///     A warning that may require attention but does not prevent processing.
    /// </summary>
    WARNING,
}
