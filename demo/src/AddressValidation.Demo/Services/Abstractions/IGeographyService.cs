namespace AddressValidation.Demo.Services.Abstractions;

using Visus.AddressValidation.Abstractions;

public interface IGeographyService
{
    ValueTask<IReadOnlyDictionary<string, string>> ListCountriesAsDictionaryAsync(params CountryCode[] supportedCountryCodes);

    ValueTask<IReadOnlyDictionary<string, string>> ListProvincesAsDictionaryAsync(string countryCode);
}
