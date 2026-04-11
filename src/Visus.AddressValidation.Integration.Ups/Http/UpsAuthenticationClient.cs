namespace Visus.AddressValidation.Integration.Ups.Http;

using System.Net.Http.Json;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Serialization.Json;
using Configuration;
using Microsoft.Extensions.Options;

internal sealed class UpsAuthenticationClient : IAuthenticationClient
{
    private readonly HttpClient _httpClient;

    private readonly IOptions<UpsServiceOptions> _options;

    public UpsAuthenticationClient(HttpClient httpClient, IOptions<UpsServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async ValueTask<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(_options.Value.ClientId) ||
             string.IsNullOrWhiteSpace(_options.Value.ClientSecret) ||
             string.IsNullOrWhiteSpace(_options.Value.AccountNumber) )
        {
            throw new InvalidOperationException($"{nameof(UpsServiceOptions.ClientId)}, {nameof(UpsServiceOptions.ClientSecret)}, and {nameof(UpsServiceOptions.AccountNumber)} are required.");
        }

        Uri baseUri = _options.Value.ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointBaseUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointBaseUri,
            _ => Constants.DevelopmentEndpointBaseUri,
        };

        Uri requestUri = new(baseUri, "/security/v1/oauth/token");

        List<KeyValuePair<string, string>> payload =
        [
            new("grant_type", "client_credentials"),
        ];

        using HttpRequestMessage request = new(HttpMethod.Post, requestUri);

        request.Content = new FormUrlEncodedContent(payload);
        request.Headers.Authorization = new BasicAuthenticationHeaderValue(_options.Value.ClientId, _options.Value.ClientSecret);
        request.Headers.Add("x-merchant-id", _options.Value.AccountNumber);

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
