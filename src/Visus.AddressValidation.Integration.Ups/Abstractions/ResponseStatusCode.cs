namespace Visus.AddressValidation.Integration.Ups.Abstractions;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter<ResponseStatusCode>))]
internal enum ResponseStatusCode
{
    FAILURE = 0,
    SUCCESS = 1
}
