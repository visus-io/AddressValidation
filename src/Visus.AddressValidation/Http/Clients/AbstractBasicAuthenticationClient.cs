namespace Visus.AddressValidation.Http.Clients;

using System.Net.Http.Json;
using Http;
using Serialization.Json;

/// <summary>
///     Abstract base class for an authentication client that obtains access tokens
///     using the OAuth 2.0 client credentials grant with HTTP Basic Authentication.
/// </summary>
public abstract class AbstractBasicAuthenticationClient : IAuthenticationClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractBasicAuthenticationClient" />.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient" /> used to send token requests.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="httpClient" /> is <see langword="null" />.
    /// </exception>
    protected AbstractBasicAuthenticationClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    ///     Gets the password (client secret or API secret) used for Basic Authentication.
    /// </summary>
    protected abstract string Password { get; }

    /// <summary>
    ///     Gets the full URI of the token endpoint.
    /// </summary>
    protected abstract Uri TokenUri { get; }

    /// <summary>
    ///     Gets the username (client ID or API key) used for Basic Authentication.
    /// </summary>
    protected abstract string Username { get; }

    /// <inheritdoc />
    public async Task<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        List<KeyValuePair<string, string>> payload =
        [
            new("grant_type", "client_credentials"),
        ];

        using HttpRequestMessage request = new(HttpMethod.Post, TokenUri);

        request.Content = new FormUrlEncodedContent(payload);
        request.Headers.Authorization = new BasicAuthenticationHeaderValue(Username, Password);

        AddAdditionalHeaders(request);

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(DefaultJsonSerializerContext.Default.TokenResponse,
                                  cancellationToken)
                             .ConfigureAwait(false);
    }

    /// <summary>
    ///     Adds provider-specific headers to the token request. The default implementation is a no-op.
    /// </summary>
    /// <param name="request">The outgoing <see cref="HttpRequestMessage" /> to augment.</param>
    protected virtual void AddAdditionalHeaders(HttpRequestMessage request)
    {
    }
}
