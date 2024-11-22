namespace Visus.AddressValidation.Integration.Google.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(GoogleAddressValidationRequest))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class GoogleJsonSerializerContext : JsonSerializerContext;
