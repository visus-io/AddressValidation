namespace Visus.AddressValidation.Integration.PitneyBowes.Abstractions;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[JsonConverter(typeof(JsonStringEnumConverter<StatusCode>))]
internal enum StatusCode
{
    VALIDATED_CHANGED,
    VALIDATED_AND_NOT_CHANGED,
    NOT_CHANGED,
}
