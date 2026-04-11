namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using System.Net.Http.Json;
using AddressValidation.Http;
using AddressValidation.Serialization.Json;
using Configuration;
using Microsoft.Extensions.Options;

internal sealed class PitneyBowesAuthenticationClient : IAuthenticationClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<PitneyBowesServiceOptions> _options;

    public PitneyBowesAuthenticationClient(HttpClient httpClient, IOptions<PitneyBowesServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async ValueTask<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(_options.Value.ApiKey)
          || string.IsNullOrWhiteSpace(_options.Value.ApiSecret)
          || string.IsNullOrWhiteSpace(_options.Value.DeveloperId) )
        {
            throw new InvalidOperationException($"{nameof(PitneyBowesServiceOptions.ApiKey)}, {nameof(PitneyBowesServiceOptions.ApiSecret)} and {nameof(PitneyBowesServiceOptions.DeveloperId)} are required.");
        }

        Uri requestUri = new(_options.Value.EndpointBaseUri, "/oauth/token");

        List<KeyValuePair<string, string>> payload =
        [
            new("grant_type", "client_credentials"),
        ];

        using HttpRequestMessage request = new(HttpMethod.Post, requestUri);

        request.Content = new FormUrlEncodedContent(payload);
        request.Headers.Authorization = new BasicAuthenticationHeaderValue(_options.Value.ApiKey, _options.Value.ApiSecret);

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
