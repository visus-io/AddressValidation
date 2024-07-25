namespace Visus.AddressValidation.Ups.Http;

using Abstractions;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

public sealed class UpsAuthenticationService : IAuthenticationService
{
	private const string AccountNumberConfigurationKey = "VS_AVE_UPS_ACCOUNT_NUMBER";

	private const string ClientEnvironment = "VS_AVE_UPS_CLIENT_ENVIRONMENT";

	private const string ClientIdConfigurationKey = "VS_AVE_UPS_CLIENT_ID";

	private const string ClientSecretConfigurationKey = "VS_AVE_UPS_CLIENT_SECRET";

	private readonly IMemoryCache _cache;

	private readonly IConfiguration _configuration;

	private readonly IHttpClientFactory _httpClientFactory;

	private readonly string? CacheKey;

	public UpsAuthenticationService(IMemoryCache cache,
									IConfiguration configuration,
									IHttpClientFactory httpClientFactory)
	{
		_cache = cache ?? throw new ArgumentNullException(nameof(cache));
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		string? accountNumber = configuration[AccountNumberConfigurationKey];
		if ( string.IsNullOrWhiteSpace(accountNumber) )
		{
			throw new InvalidOperationException();
		}

		if ( !Enum.TryParse(configuration[ClientEnvironment], out ClientEnvironment clientEnvironment) )
		{
			clientEnvironment = Abstractions.ClientEnvironment.PRODUCTION;
		}

		CacheKey = $"VS_AVE_CACHE_UPS_ACCESS_TOKEN_${accountNumber}:{clientEnvironment}";
	}

	public async ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
	{
		if ( string.IsNullOrWhiteSpace(CacheKey) )
		{
			return null;
		}

		string? accessToken = _cache.Get<string?>(CacheKey);
		if ( !string.IsNullOrWhiteSpace(accessToken) )
		{
			return accessToken;
		}

		HttpClient client = _httpClientFactory.CreateClient("");

		ClientCredentialsTokenRequest request = new()
		{
			ClientId = _configuration[ClientIdConfigurationKey]!,
			ClientSecret = _configuration[ClientSecretConfigurationKey]!
		};

		request.Headers.Add("x-merchant-id", _configuration[AccountNumberConfigurationKey]);

		TokenResponse response = await client.RequestClientCredentialsTokenAsync(request, cancellationToken);
		if ( string.IsNullOrWhiteSpace(response.AccessToken) )
		{
			return null;
		}

		DateTimeOffset expires = DateTimeOffset.UtcNow;
		expires = expires.AddSeconds(response.ExpiresIn);

		_cache.Set(CacheKey, response.AccessToken, expires);

		return response.AccessToken;
	}
}
