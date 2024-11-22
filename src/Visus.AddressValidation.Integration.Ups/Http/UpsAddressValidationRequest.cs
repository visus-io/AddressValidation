namespace Visus.AddressValidation.Integration.Ups.Http;

using System.Text.Json.Serialization;
using AddressValidation.Http;
using Serialization.Json;

/// <summary>
///     Representation of a uniformed address validation request to UPS.
/// </summary>
[JsonConverter(typeof(AddressValidationRequestConverter))]
public sealed class UpsAddressValidationRequest : AbstractAddressValidationRequest;
