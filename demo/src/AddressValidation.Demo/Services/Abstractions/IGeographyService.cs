namespace AddressValidation.Demo.Services.Abstractions;

using Visus.AddressValidation.Abstractions;

public interface IGeographyService
{
    ValueTask<IReadOnlySet<string>> ListAutonomousCitiesAsync(string countryCode);
    
    ValueTask<IReadOnlyDictionary<string, string>> ListCountriesAsDictionaryAsync(params CountryCode[] countryCodes);

    ValueTask<IReadOnlyDictionary<string, string>> ListProvincesAsDictionaryAsync(string countryCode);
}
