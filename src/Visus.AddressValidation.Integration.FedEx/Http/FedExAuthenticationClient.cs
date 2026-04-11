namespace Visus.AddressValidation.Integration.FedEx.Http;

using System.Net.Http.Json;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Serialization.Json;
using Configuration;
using Microsoft.Extensions.Options;

internal sealed class FedExAuthenticationClient : IAuthenticationClient
{
    private readonly HttpClient _httpClient;

    private readonly IOptions<FedExServiceOptions> _options;

    public FedExAuthenticationClient(HttpClient httpClient, IOptions<FedExServiceOptions> options)
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
            throw new InvalidOperationException($"{nameof(FedExServiceOptions.ClientId)}, {nameof(FedExServiceOptions.ClientSecret)}, and {nameof(_options.Value.AccountNumber)} are required.");
        }
        
        Uri baseUri = _options.Value.ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointBaseUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointBaseUri,
            _ => Constants.DevelopmentEndpointBaseUri,
        };

        Uri requestUri = new(baseUri, "/oauth/token");

        List<KeyValuePair<string, string>> payload =
        [
            new("client_id", _options.Value.ClientId),
            new("client_secret", _options.Value.ClientSecret),
            new("grant_type", "client_credentials"),
        ];

        using HttpRequestMessage request = new(HttpMethod.Post, requestUri);

        request.Content = new FormUrlEncodedContent(payload);

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
