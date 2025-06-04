namespace Visus.AddressValidation.Services;

using Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
///     Abstraction for implementing an authentication service that relies on an
///     <see cref="IAuthenticationClient" /> implementation.
/// </summary>
/// <param name="authenticationClient">An <see cref="IAuthenticationClient" /> instance.</param>
/// <param name="cache">An <see cref="IMemoryCache" /> instance.</param>
/// <typeparam name="TClient">
///     An instance that implements the <see cref="IAuthenticationClient" />
///     interface.
/// </typeparam>
public abstract class AbstractAuthenticationService<TClient>(TClient authenticationClient, IDistributedCache cache)
    where TClient : IAuthenticationClient
{
    private readonly TClient _authenticationClient = authenticationClient ?? throw new ArgumentNullException(nameof(authenticationClient));

    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private string? _cacheKey;

    /// <summary>
    ///     Key used by the underlying cache service to retrieve the cached access token.
    /// </summary>
    public string? CacheKey
    {
        get
        {
            if ( string.IsNullOrWhiteSpace(_cacheKey) )
            {
                _cacheKey = GenerateCacheKey();
            }

            return _cacheKey;
        }
    }

    /// <summary>
    ///     Retrieves an access token (token-based authentication).
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     Current access token returned by the service, an empty string, or <see langword="null" />.
    /// </returns>
    /// <remarks>An instance of <see cref="IMemoryCache" /> is utilized to cache the token by its defined lifetime.</remarks>
    public async ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(CacheKey) )
        {
            return null;
        }

        string? accessToken = await _cache.GetStringAsync(CacheKey, cancellationToken).ConfigureAwait(false);
        if ( !string.IsNullOrWhiteSpace(accessToken) )
        {
            return accessToken;
        }

        TokenResponse? response = await _authenticationClient.RequestClientCredentialsTokenAsync(cancellationToken).ConfigureAwait(false);
        if ( response is null || string.IsNullOrWhiteSpace(response.AccessToken) )
        {
            return null;
        }

        DateTimeOffset expires = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn - 60);

        await _cache.SetStringAsync(CacheKey, response.AccessToken, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = expires
        }, cancellationToken).ConfigureAwait(false);

        return response.AccessToken;
    }

    /// <summary>
    ///     Generates a unique cache key for caching retrieved access tokens.
    /// </summary>
    /// <returns>Generated unique key value or <see langword="null" />.</returns>
    protected abstract string? GenerateCacheKey();
}
