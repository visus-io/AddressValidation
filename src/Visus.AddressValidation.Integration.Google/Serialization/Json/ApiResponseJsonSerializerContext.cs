namespace Visus.AddressValidation.Integration.Google.Serialization.Json;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Contracts;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class ApiResponseJsonSerializerContext : JsonSerializerContext;
