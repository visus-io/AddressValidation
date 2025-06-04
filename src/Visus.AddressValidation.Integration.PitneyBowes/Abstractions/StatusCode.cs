namespace Visus.AddressValidation.Integration.PitneyBowes.Abstractions;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum StatusCode
{
    VALIDATED_CHANGED,
    VALIDATED_AND_NOT_CHANGED,
    NOT_CHANGED
}
