namespace Visus.AddressValidation.Services;

using Http;
using Http.Clients;
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
    /// <summary>
    ///     Prefix applied to all cache keys produced by this service to namespace them within the shared cache.
    /// </summary>
    protected const string CacheKeyTag = "vs-ave-auth:";

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

    private string CacheKey
    {
        get
        {
            if ( !string.IsNullOrWhiteSpace(field) )
            {
                return field;
            }

            string key = GenerateCacheKey();

            if ( !IsValidCacheKey(key) )
            {
                throw new InvalidOperationException(
                    $"Cache key '{key}' returned by {nameof(GenerateCacheKey)} contains invalid characters. "
                  + "Keys must only contain A-Z, a-z, 0-9, underscores, hyphens, and colons.");
            }

            field = key;

            return field;
        }
    }

    /// <summary>
    ///     Retrieves an access token using token-based authentication.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     The current access token returned by the authentication service, or <see langword="null" /> if
    ///     the authentication service did not return a valid token.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         The access token is cached using <see cref="HybridCache" />. When no cached entry exists,
    ///         a new token is fetched via <see cref="IAuthenticationClient.RequestClientCredentialsTokenAsync" />
    ///         and stored in the cache with an expiration of <c>ExpiresIn - 60</c> seconds to provide a
    ///         safety buffer before the token actually expires.
    ///     </para>
    ///     <para>
    ///         Subsequent calls return the cached token until it expires, at which point a new token is
    ///         fetched and cached. If the authentication service returns an invalid or empty token, the
    ///         cache entry is removed and <see langword="null" /> is returned.
    ///     </para>
    /// </remarks>
    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        bool factoryRan = false;
        TokenResponse? fetched = null;

        TokenResponse? tokenResponse = await _cache.GetOrCreateAsync<TokenResponse?>(CacheKey,
                                                        async ct =>
                                                        {
                                                            factoryRan = true;

                                                            fetched = await _authenticationClient
                                                                           .RequestClientCredentialsTokenAsync(ct)
                                                                           .ConfigureAwait(false);

                                                            return fetched;
                                                        },
                                                        null,
                                                        null,
                                                        cancellationToken)
                                                   .ConfigureAwait(false);

        if ( !factoryRan ) // NOSONAR S2583 - false positive: factoryRan is mutated inside the async lambda above
        {
            return string.IsNullOrWhiteSpace(tokenResponse?.AccessToken) ? null : tokenResponse.AccessToken;
        }

        if ( fetched is null || string.IsNullOrWhiteSpace(fetched.AccessToken) )
        {
            await _cache.RemoveAsync(CacheKey, cancellationToken).ConfigureAwait(false);
            return null;
        }

        await _cache.SetAsync(CacheKey,
            fetched,
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
    /// <returns>
    ///     A non-null, non-empty string containing only A-Z, a-z, 0-9, underscores, hyphens, and colons.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the required credentials needed to construct the key are absent or invalid,
    ///     or when the returned key contains characters outside the allowed set.
    /// </exception>
    protected abstract string GenerateCacheKey();

    private static bool IsValidCacheKey(string key)
    {
        return key.Length > 0 && key.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-' or ':');
    }
}
