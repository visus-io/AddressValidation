namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

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
    ///     Contains Business & Residential Units (Multi-Tenant Only)
    /// </summary>
    MIXED,

    /// <summary>
    ///     Residential Address
    /// </summary>
    RESIDENTIAL
}
