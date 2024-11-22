namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(UpsAddressValidationRequest))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class UpsJsonSerializerContext : JsonSerializerContext;
