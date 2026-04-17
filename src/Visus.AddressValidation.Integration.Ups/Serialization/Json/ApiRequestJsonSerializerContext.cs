namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(ApiRequest))]
[JsonSourceGenerationOptions(WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal sealed partial class ApiRequestJsonSerializerContext : JsonSerializerContext;
