namespace AddressValidation.Demo.Services;

using Abstractions;
using Infrastructure.Repositories.Abstractions;
using Models.Entities;
using Visus.AddressValidation.Abstractions;

public sealed class GeographyService(ICountryRepository countryRepository, IStateRepository stateRepository) : IGeographyService
{
	private readonly ICountryRepository _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));

	private readonly IStateRepository _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));

	public async ValueTask<IReadOnlyDictionary<string, string>> ListCountriesAsDictionaryAsync(params CountryCode[] supportedCountryCodes)
	{
		ArgumentNullException.ThrowIfNull(supportedCountryCodes);

		HashSet<string> countryCodes = new(StringComparer.OrdinalIgnoreCase);
		if ( supportedCountryCodes.Length > 0 )
		{
			foreach ( CountryCode countryCode in supportedCountryCodes )
			{
				countryCodes.Add(countryCode.ToString());
			}
		}

		IReadOnlyList<CountryModel> results;
		if ( countryCodes.Count == 0 )
		{
			results = await _countryRepository.ListAsync().ConfigureAwait(false);
		}
		else
		{
			results = await _countryRepository.ListAsync(l => countryCodes.Contains(l.IsoCode)).ConfigureAwait(false);
		}

		return results.OrderBy(o => o.Name)
					  .ToDictionary(country => country.IsoCode, country => country.Name, StringComparer.OrdinalIgnoreCase);
	}

	public ValueTask<IReadOnlyDictionary<string, string>> ListProvincesAsDictionaryAsync(string countryCode)
	{
		if ( !Enum.TryParse(countryCode, true, out CountryCode _) )
		{
			throw new ArgumentException($"'{countryCode}' is not a valid country code.", nameof(countryCode));
		}

		return ListProvincesAsDictionaryInternalAsync(countryCode);
	}

	private async ValueTask<IReadOnlyDictionary<string, string>> ListProvincesAsDictionaryInternalAsync(string countryCode)
	{
		IReadOnlyList<StateModel> results = await _stateRepository.ListAsync(l => l.CountryCode == countryCode).ConfigureAwait(false);
		return results.OrderBy(o => o.Name)
					  .ToDictionary(state => state.IsoCode, state => state.Name, StringComparer.OrdinalIgnoreCase);
	}
}
