namespace Visus.AddressValidation.Integration.FedEx.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(FedExAddressValidationRequest))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class FedExJsonSerializerContext : JsonSerializerContext;
