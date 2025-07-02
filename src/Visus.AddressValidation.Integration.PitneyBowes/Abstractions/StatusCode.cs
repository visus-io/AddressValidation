namespace Visus.AddressValidation.Integration.PitneyBowes.Abstractions;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[JsonConverter(typeof(JsonStringEnumConverter<StatusCode>))]
internal enum StatusCode
{
    VALIDATED_CHANGED,
    VALIDATED_AND_NOT_CHANGED,
    NOT_CHANGED
}
