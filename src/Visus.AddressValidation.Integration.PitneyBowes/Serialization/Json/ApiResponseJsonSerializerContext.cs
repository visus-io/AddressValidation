namespace Visus.AddressValidation.Integration.PitneyBowes.Serialization.Json;

using Contracts;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(ApiResponse.AddressResult))]
[JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class ApiResponseJsonSerializerContext : JsonSerializerContext;
