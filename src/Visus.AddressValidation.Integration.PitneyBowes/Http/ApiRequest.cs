namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using AddressValidation.Abstractions;

internal sealed class ApiRequest
{
    public required IReadOnlyList<string> AddressLines { get; init; }

    public string? CityTown { get; init; }

    public required CountryCode CountryCode { get; init; }

    public required string PostalCode { get; init; }

    public string? StateProvince { get; init; }
}
