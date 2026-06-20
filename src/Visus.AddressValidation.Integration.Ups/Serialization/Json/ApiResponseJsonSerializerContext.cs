namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using Contracts;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSourceGenerationOptions(WriteIndented = false)]
internal sealed partial class ApiResponseJsonSerializerContext : JsonSerializerContext;
