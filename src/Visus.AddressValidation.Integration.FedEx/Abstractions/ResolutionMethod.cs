namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

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
    ///     US address geocoded using NAVTEQ map data.
    /// </summary>
    NAVTEQ_GEO_VALIDATE,

    /// <summary>
    ///     US address geocoded using TeleAtlas map data.
    /// </summary>
    TELEATLAS_GEO_VALIDATE,
}
