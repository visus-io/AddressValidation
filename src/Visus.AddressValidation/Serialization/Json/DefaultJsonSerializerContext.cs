namespace Visus.AddressValidation.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(TokenResponse))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class DefaultJsonSerializerContext : JsonSerializerContext;
