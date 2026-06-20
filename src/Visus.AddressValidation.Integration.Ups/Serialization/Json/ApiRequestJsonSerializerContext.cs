namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using Contracts;

[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(ApiRequest))]
[JsonSourceGenerationOptions(WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal sealed partial class ApiRequestJsonSerializerContext : JsonSerializerContext;
