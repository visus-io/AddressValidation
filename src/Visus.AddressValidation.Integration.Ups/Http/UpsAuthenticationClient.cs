namespace Visus.AddressValidation.Integration.Ups.Http;

using System.Net.Http.Json;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Serialization.Json;
using Microsoft.Extensions.Configuration;

internal sealed class UpsAuthenticationClient : IAuthenticationClient
{
    private readonly IConfiguration _configuration;

    private readonly HttpClient _httpClient;

    public UpsAuthenticationClient(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        string? clientId = _configuration[Constants.ClientIdConfigurationKey];
        string? clientSecret = _configuration[Constants.ClientSecretConfigurationKey];
        string? accountNumber = _configuration[Constants.AccountNumberConfigurationKey];

        if ( string.IsNullOrWhiteSpace(clientId) ||
             string.IsNullOrWhiteSpace(clientSecret) ||
             string.IsNullOrWhiteSpace(accountNumber) )
        {
            throw new InvalidOperationException($"{Constants.ClientIdConfigurationKey}, {Constants.ClientSecretConfigurationKey}, and {Constants.AccountNumberConfigurationKey} are required.");
        }

        if ( !Enum.TryParse(_configuration[Constants.ClientEnvironmentConfigurationKey], out ClientEnvironment clientEnvironment) )
        {
            clientEnvironment = ClientEnvironment.DEVELOPMENT;
        }

        Uri baseUri = clientEnvironment switch
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
        request.Headers.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
        request.Headers.Add("x-merchant-id", accountNumber);

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
