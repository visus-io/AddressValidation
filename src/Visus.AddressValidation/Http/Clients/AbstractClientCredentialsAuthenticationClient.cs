namespace Visus.AddressValidation.Http.Clients;

using System.Net.Http.Json;
using Serialization.Json;

/// <summary>
///     Abstract base class for an authentication client that obtains access tokens
///     using the OAuth 2.0 client credentials grant.
/// </summary>
public abstract class AbstractClientCredentialsAuthenticationClient : IAuthenticationClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractClientCredentialsAuthenticationClient" />.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient" /> used to send token requests.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="httpClient" /> is <see langword="null" />.
    /// </exception>
    protected AbstractClientCredentialsAuthenticationClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    ///     Gets the client ID used to authenticate the token request.
    /// </summary>
    protected abstract string ClientId { get; }

    /// <summary>
    ///     Gets the client secret used to authenticate the token request.
    /// </summary>
    protected abstract string ClientSecret { get; }

    /// <summary>
    ///     Gets the full URI of the token endpoint.
    /// </summary>
    protected abstract Uri TokenUri { get; }

    /// <summary>
    ///     Gets a value indicating whether <see cref="ClientId" /> and <see cref="ClientSecret" /> are sent as an
    ///     <c>Authorization: Basic</c> header. When <see langword="false" />, they are sent as <c>client_id</c> and
    ///     <c>client_secret</c> form fields in the request body instead. The default implementation returns
    ///     <see langword="false" />.
    /// </summary>
    protected virtual bool UseHttpBasicAuthentication => false;

    /// <inheritdoc />
    public async Task<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        List<KeyValuePair<string, string>> payload = [new("grant_type", "client_credentials"),];

        using HttpRequestMessage request = new(HttpMethod.Post, TokenUri);

        if ( UseHttpBasicAuthentication )
        {
            request.Headers.Authorization = new BasicAuthenticationHeaderValue(ClientId, ClientSecret);
        }
        else
        {
            payload.Add(new KeyValuePair<string, string>("client_id", ClientId));
            payload.Add(new KeyValuePair<string, string>("client_secret", ClientSecret));
        }

        request.Content = new FormUrlEncodedContent(payload);

        ApplyAdditionalHeaders(request);

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(DefaultJsonSerializerContext.Default.TokenResponse,
                                  cancellationToken)
                             .ConfigureAwait(false);
    }

    /// <summary>
    ///     Applies provider-specific headers to the token request.
    /// </summary>
    /// <param name="request">The outgoing <see cref="HttpRequestMessage" /> to augment.</param>
    protected virtual void ApplyAdditionalHeaders(HttpRequestMessage request)
    {
    }
}
