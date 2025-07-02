namespace Visus.AddressValidation.Integration.Ups.Abstractions;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter<AddressClassificationCode>))]
internal enum AddressClassificationCode
{
    UNCLASSIFIED = 0,
    COMMERCIAL = 1,
    RESIDENTIAL = 2
}
