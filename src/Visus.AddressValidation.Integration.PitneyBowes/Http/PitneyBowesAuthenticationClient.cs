namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using System.Net.Http.Json;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Serialization.Json;
using Microsoft.Extensions.Configuration;

internal sealed class PitneyBowesAuthenticationClient(IConfiguration configuration, HttpClient httpClient)
	: IAuthenticationClient
{
	private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

	private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

	public async ValueTask<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
	{
		string? apiKey = _configuration[Constants.ApiKeyConfigurationKey];
		string? apiSecret = _configuration[Constants.ApiSecretConfigurationKey];

		if ( string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret) )
		{
			throw new InvalidOperationException($"{Constants.ApiKeyConfigurationKey} and {Constants.ApiSecretConfigurationKey} are required.");
		}

		if ( !Enum.TryParse(_configuration[Constants.ClientEnvironmentConfigurationKey], out ClientEnvironment clientEnvironment) )
		{
			clientEnvironment = ClientEnvironment.DEVELOPMENT;
		}

		Uri baseUri = clientEnvironment switch
		{
			ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointBaseUri,
			ClientEnvironment.PRODUCTION => Constants.ProductionEndpointBaseUri,
			_ => Constants.DevelopmentEndpointBaseUri
		};

		Uri requestUri = new(baseUri, "/oauth/token");

		List<KeyValuePair<string, string>> payload =
		[
			new("grant_type", "client_credentials")
		];

		using HttpRequestMessage request = new(HttpMethod.Post, requestUri);

		request.Content = new FormUrlEncodedContent(payload);
		request.Headers.Authorization = new BasicAuthenticationHeaderValue(apiKey, apiSecret);

		using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
		if ( !response.IsSuccessStatusCode )
		{
			return null;
		}

		return await response.Content.ReadFromJsonAsync(DefaultJsonSerializerContext.Default.TokenResponse,
														cancellationToken)
							 .ConfigureAwait(false);
	}
}
