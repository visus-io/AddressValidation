namespace Visus.AddressValidation.Integration.PitneyBowes.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

/// <summary>
///     A source-generated <see cref="JsonSerializerContext" /> for Pitney Bowes
///     address validation JSON serialization.
/// </summary>
/// <remarks>
///     This context registers <see cref="PitneyBowesAddressValidationRequest" />
///     for source-generated serialization using compact (non-indented) output.
/// </remarks>
[JsonSerializable(typeof(PitneyBowesAddressValidationRequest))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class PitneyBowesJsonSerializerContext : JsonSerializerContext;
