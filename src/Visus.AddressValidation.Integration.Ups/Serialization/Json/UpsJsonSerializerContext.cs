namespace Visus.AddressValidation.Integration.Ups.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

/// <summary>
///     A source-generated <see cref="JsonSerializerContext" /> for UPS address
///     validation JSON serialization.
/// </summary>
/// <remarks>
///     This context registers <see cref="UpsAddressValidationRequest" /> for
///     source-generated serialization using compact (non-indented) output.
/// </remarks>
[JsonSerializable(typeof(UpsAddressValidationRequest))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class UpsJsonSerializerContext : JsonSerializerContext;
