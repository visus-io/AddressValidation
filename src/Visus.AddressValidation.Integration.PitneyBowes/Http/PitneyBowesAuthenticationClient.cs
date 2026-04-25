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

    public async Task<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        Uri requestUri = new(_options.Value.EndpointBaseUri, "/oauth/token");

        List<KeyValuePair<string, string>> payload =
        [
            new("grant_type", "client_credentials"),
        ];

        using HttpRequestMessage request = new(HttpMethod.Post, requestUri);

        request.Content = new FormUrlEncodedContent(payload);
        request.Headers.Authorization = new BasicAuthenticationHeaderValue(_options.Value.ApiKey, _options.Value.ApiSecret);

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(DefaultJsonSerializerContext.Default.TokenResponse,
                                  cancellationToken)
                             .ConfigureAwait(false);
    }
}
