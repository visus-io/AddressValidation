namespace AddressValidation.Demo.Services;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Abstractions;
using Infrastructure.Repositories.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Models.Entities;
using Visus.AddressValidation.Abstractions;

public sealed class GeographyService(
	IDistributedCache cache,
	ICountryRepository countryRepository,
	IStateRepository stateRepository) : IGeographyService
{
	private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

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

		string cacheKey = await GenerateCacheKeyAsync("countries", countryCodes).ConfigureAwait(false);

		string? response = await _cache.GetStringAsync(cacheKey).ConfigureAwait(false);
		if ( !string.IsNullOrWhiteSpace(response) )
		{
			return JsonSerializer.Deserialize<IReadOnlyDictionary<string, string>>(response)!;
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

		Dictionary<string, string> dict = results.OrderBy(o => o.Name)
												 .ToDictionary(country => country.IsoCode,
															   country => country.Name,
															   StringComparer.OrdinalIgnoreCase);

		await _cache.SetStringAsync(cacheKey,
									JsonSerializer.Serialize(dict),
									new DistributedCacheEntryOptions
									{
										AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
									}).ConfigureAwait(false);

		return dict;
	}

	public ValueTask<IReadOnlyDictionary<string, string>> ListProvincesAsDictionaryAsync(string countryCode)
	{
		if ( !Enum.TryParse(countryCode, true, out CountryCode _) )
		{
			throw new ArgumentException($"'{countryCode}' is not a valid country code.", nameof(countryCode));
		}

		return ListProvincesAsDictionaryInternalAsync(countryCode);
	}

	private static async ValueTask<string> GenerateCacheKeyAsync(string prefix, IReadOnlySet<string> values)
	{
		using SHA256 sha = SHA256.Create();

		byte[] checksum = await sha.ComputeHashAsync(Stream.Null).ConfigureAwait(false);
		if ( values.Count == 0 )
		{
			return prefix + "_" + BitConverter.ToString(checksum).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
		}

		string content = string.Join(";", values);
		using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));

		checksum = await sha.ComputeHashAsync(stream).ConfigureAwait(false);

		return prefix + "_" + BitConverter.ToString(checksum).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
	}

	private async ValueTask<IReadOnlyDictionary<string, string>> ListProvincesAsDictionaryInternalAsync(string countryCode)
	{
		string cacheKey = await GenerateCacheKeyAsync("provinces", new HashSet<string>
		{
			countryCode
		}).ConfigureAwait(false);

		string? response = await _cache.GetStringAsync(cacheKey).ConfigureAwait(false);
		if ( !string.IsNullOrWhiteSpace(response) )
		{
			return JsonSerializer.Deserialize<IReadOnlyDictionary<string, string>>(response)!;
		}

		IReadOnlyList<StateModel> results = await _stateRepository.ListAsync(l => l.CountryCode == countryCode)
																  .ConfigureAwait(false);

		Dictionary<string, string> dict = results.OrderBy(o => o.Name)
												 .ToDictionary(state => state.IsoCode,
															   state => state.Name,
															   StringComparer.OrdinalIgnoreCase);

		await _cache.SetStringAsync(cacheKey,
									JsonSerializer.Serialize(dict),
									new DistributedCacheEntryOptions
									{
										AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
									}).ConfigureAwait(false);

		return dict;
	}
}
