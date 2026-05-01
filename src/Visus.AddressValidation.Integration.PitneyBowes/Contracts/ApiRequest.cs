namespace Visus.AddressValidation.Integration.PitneyBowes.Contracts;

using System.Text.Json.Serialization;
using AddressValidation.Abstractions;

internal sealed class ApiRequest
{
    public required IReadOnlyList<string> AddressLines { get; init; }

    public string? CityTown { get; init; }

    public required CountryCode CountryCode { get; init; }

    [JsonIgnore]
    public required bool IncludeSuggestions { get; init; }

    public required string PostalCode { get; init; }

    public string? StateProvince { get; init; }
}
