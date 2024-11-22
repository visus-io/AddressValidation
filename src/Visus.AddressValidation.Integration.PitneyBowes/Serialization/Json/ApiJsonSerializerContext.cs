namespace Visus.AddressValidation.Integration.PitneyBowes.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(ApiResponse.AddressResult))]
[JsonSourceGenerationOptions(WriteIndented = false)]
internal partial class ApiJsonSerializerContext : JsonSerializerContext;
