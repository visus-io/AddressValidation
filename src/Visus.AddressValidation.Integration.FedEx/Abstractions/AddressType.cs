namespace Visus.AddressValidation.Integration.FedEx.Abstractions;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[JsonConverter(typeof(JsonStringEnumConverter<AddressType>))]
internal enum AddressType
{
    /// <summary>
    ///     Address country not supported
    /// </summary>
    RAW,

    /// <summary>
    ///     Address country supported, but unable to match the address against reference data.
    /// </summary>
    NORMAL,

    /// <summary>
    ///     Address service was able to successfully match the address against reference data.
    /// </summary>
    STANDARDIZED,
}
