namespace Visus.AddressValidation.Serialization.Json;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Http;

/// <summary>
///     Default <see cref="System.Text.Json.Serialization.JsonSerializerContext" /> for the library.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(TokenResponse))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class DefaultJsonSerializerContext : JsonSerializerContext;
