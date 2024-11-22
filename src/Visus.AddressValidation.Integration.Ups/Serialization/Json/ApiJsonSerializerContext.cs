namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSourceGenerationOptions(WriteIndented = false)]
internal partial class ApiJsonSerializerContext : JsonSerializerContext;
