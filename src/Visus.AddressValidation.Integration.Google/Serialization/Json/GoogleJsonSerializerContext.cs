namespace Visus.AddressValidation.Integration.Google.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

/// <summary>
///     A source-generated <see cref="JsonSerializerContext" /> for Google address
///     validation JSON serialization.
/// </summary>
/// <remarks>
///     This context registers <see cref="GoogleAddressValidationRequest" /> for
///     source-generated serialization using compact (non-indented) output.
/// </remarks>
[JsonSerializable(typeof(GoogleAddressValidationRequest))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class GoogleJsonSerializerContext : JsonSerializerContext;
