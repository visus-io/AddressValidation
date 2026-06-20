namespace Visus.AddressValidation.Integration.Ups.Abstractions;

[JsonConverter(typeof(JsonStringEnumConverter<ResponseStatusCode>))]
internal enum ResponseStatusCode
{
    FAILURE = 0,
    SUCCESS = 1,
}
