namespace Visus.AddressValidation.Integration.FedEx.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

/// <summary>
///     A source-generated <see cref="JsonSerializerContext" /> for FedEx address
///     validation JSON serialization.
/// </summary>
/// <remarks>
///     This context registers <see cref="FedExAddressValidationRequest" /> for
///     source-generated serialization using camel-case property naming and
///     compact (non-indented) output.
/// </remarks>
[JsonSerializable(typeof(FedExAddressValidationRequest))]
[JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class FedExJsonSerializerContext : JsonSerializerContext;
