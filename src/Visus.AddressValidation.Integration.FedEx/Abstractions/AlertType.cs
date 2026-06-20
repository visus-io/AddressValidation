namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

/// <summary>
///     Represents the type of alert returned in a FedEx API response.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<AlertType>))]
internal enum AlertType
{
    /// <summary>
    ///     An informational note that does not indicate a problem.
    /// </summary>
    NOTE,

    /// <summary>
    ///     A warning that may require attention but does not prevent processing.
    /// </summary>
    WARNING,
}
