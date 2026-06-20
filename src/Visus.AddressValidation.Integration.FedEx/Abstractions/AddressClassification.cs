namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[JsonConverter(typeof(JsonStringEnumConverter<AddressClassification>))]
internal enum AddressClassification
{
    /// <summary>
    ///     Unknown Classification
    /// </summary>
    UNKNOWN,

    /// <summary>
    ///     Business Address
    /// </summary>
    BUSINESS,

    /// <summary>
    ///     Contains Business &amp; Residential Units (Multi-Tenant Only)
    /// </summary>
    MIXED,

    /// <summary>
    ///     Residential Address
    /// </summary>
    RESIDENTIAL,
}
