namespace Visus.AddressValidation.Integration.PitneyBowes.Serialization.Json;

using System.Text.Json.Serialization;
using Http;

[JsonSerializable(typeof(PitneyBowesAddressValidationRequest))]
[JsonSourceGenerationOptions(WriteIndented = false)]
public partial class PitneyBowesJsonSerializerContext : JsonSerializerContext;
