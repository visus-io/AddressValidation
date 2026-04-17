namespace Visus.AddressValidation.Integration.Google.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(ApiRequest))]
[JsonSourceGenerationOptions(WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal sealed partial class ApiRequestJsonSerializerContext : JsonSerializerContext;
