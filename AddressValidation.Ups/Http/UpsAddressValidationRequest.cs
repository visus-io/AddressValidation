namespace Visus.AddressValidation.Ups.Http;

using System.Text.Json.Serialization;
using Serialization.Json;

/// <inheritdoc />
[JsonConverter(typeof(AddressValidationRequestConverter))]
public sealed class UpsAddressValidationRequest : AbstractAddressValidationRequest
{
}
