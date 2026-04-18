namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Http;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(ApiRequest))]
[JsonSourceGenerationOptions(WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal sealed partial class ApiRequestJsonSerializerContext : JsonSerializerContext;
