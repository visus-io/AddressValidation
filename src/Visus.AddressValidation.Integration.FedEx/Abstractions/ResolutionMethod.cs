namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[JsonConverter(typeof(JsonStringEnumConverter<ResolutionMethod>))]
internal enum ResolutionMethod
{
    /// <summary>
    ///     US Postal
    /// </summary>
    USPS_VALIDATE,

    /// <summary>
    ///     Canadian Postal
    /// </summary>
    CA_VALIDATE,

    /// <summary>
    ///     Other Validation
    /// </summary>
    GENERIC_VALIDATE,

    /// <summary>
    ///     US Geo/Map Validated
    /// </summary>
    NAVTEQ_GEO_VALIDATE,

    /// <summary>
    ///     US Geo/Map Validated
    /// </summary>
    TELEATLAS_GEO_VALIDATE
}
