namespace Visus.AddressValidation.Integration.FedEx.Serialization.Json;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Http;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class ApiResponseJsonSerializerContext : JsonSerializerContext;
