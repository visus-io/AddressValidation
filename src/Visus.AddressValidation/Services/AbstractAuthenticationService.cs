namespace Visus.AddressValidation.Services;

using Http;
using Microsoft.Extensions.Caching.Hybrid;

/// <summary>
///     Abstract base class for an authentication service that obtains and caches access tokens
///     via an <see cref="IAuthenticationClient" /> implementation.
/// </summary>
/// <typeparam name="TClient">
///     The type of <see cref="IAuthenticationClient" /> used to request access tokens.
/// </typeparam>
public abstract class AbstractAuthenticationService<TClient> where TClient : IAuthenticationClient
{
    private readonly TClient _authenticationClient;

    private readonly HybridCache _cache;

    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractAuthenticationService{TClient}" />.
    /// </summary>
    /// <param name="authenticationClient">
    ///     The <see cref="IAuthenticationClient" /> used to request access tokens.
    /// </param>
    /// <param name="cache">
    ///     The <see cref="HybridCache" /> used to cache access tokens.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="authenticationClient" /> or <paramref name="cache" /> is
    ///     <see langword="null" />.
    /// </exception>
    protected AbstractAuthenticationService(TClient authenticationClient, HybridCache cache)
    {
        _authenticationClient = authenticationClient ?? throw new ArgumentNullException(nameof(authenticationClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    ///     Key used by the underlying cache service to retrieve the cached access token.
    /// </summary>
    public string? CacheKey
    {
        get
        {
            if ( string.IsNullOrWhiteSpace(field) )
            {
                field = GenerateCacheKey();
            }

            return field;
        }
    }

    /// <summary>
    ///     Retrieves an access token using token-based authentication.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     The current access token returned by the authentication service, or <see langword="null" /> if
    ///     <see cref="CacheKey" /> is <see langword="null" /> or empty, or if the authentication service
    ///     did not return a valid token.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         The access token is cached using <see cref="HybridCache" />. On first call, the token is
    ///         fetched via <see cref="IAuthenticationClient.RequestClientCredentialsTokenAsync" /> and stored
    ///         in the cache with an expiration derived from the token's lifetime, reduced by 60 seconds to
    ///         account for clock skew.
    ///     </para>
    ///     <para>
    ///         Subsequent calls return the cached token until it expires, at which point a new token is
    ///         fetched. If the authentication service returns an invalid or empty token, the cache entry
    ///         is removed and <see langword="null" /> is returned.
    ///     </para>
    /// </remarks>
    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(CacheKey) )
        {
            return null;
        }

        bool factoryRan = false;
        TokenResponse? fetched = null;

        string? accessToken = await _cache.GetOrCreateAsync<string?>(CacheKey,
                                               async ct =>
                                               {
                                                   factoryRan = true;

                                                   fetched = await _authenticationClient
                                                                  .RequestClientCredentialsTokenAsync(ct)
                                                                  .ConfigureAwait(false);

                                                   return string.IsNullOrWhiteSpace(fetched?.AccessToken)
                                                              ? null
                                                              : fetched.AccessToken;
                                               },
                                               null,
                                               null,
                                               cancellationToken)
                                          .ConfigureAwait(false);

        if ( !factoryRan )
        {
            return string.IsNullOrWhiteSpace(accessToken) ? null : accessToken;
        }

        if ( fetched is null || string.IsNullOrWhiteSpace(fetched.AccessToken) )
        {
            await _cache.RemoveAsync(CacheKey, cancellationToken).ConfigureAwait(false);
            return null;
        }

        await _cache.SetAsync(CacheKey,
            fetched.AccessToken,
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(fetched.ExpiresIn - 60),
            },
            null,
            cancellationToken).ConfigureAwait(false);

        return fetched.AccessToken;
    }

    /// <summary>
    ///     Generates a unique cache key for caching retrieved access tokens.
    /// </summary>
    /// <returns>Generated unique key value or <see langword="null" />.</returns>
    protected abstract string? GenerateCacheKey();
}
