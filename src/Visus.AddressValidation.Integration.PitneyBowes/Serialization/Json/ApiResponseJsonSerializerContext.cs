namespace Visus.AddressValidation.Integration.PitneyBowes.Serialization.Json;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Contracts;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(ApiResponse.AddressResult))]
[JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class ApiResponseJsonSerializerContext : JsonSerializerContext;
