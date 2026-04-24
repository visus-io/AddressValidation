namespace Visus.AddressValidation.Services;

using Http;
using Microsoft.Extensions.Caching.Hybrid;

/// <summary>
///     Abstraction for implementing an authentication service that relies on an
///     <see cref="IAuthenticationClient" /> implementation.
/// </summary>
/// <typeparam name="TClient">
///     An instance that implements the <see cref="IAuthenticationClient" />
///     interface.
/// </typeparam>
public abstract class AbstractAuthenticationService<TClient> where TClient : IAuthenticationClient
{
    private readonly TClient _authenticationClient;

    private readonly HybridCache _cache;

    /// <summary>
    ///     Abstraction for implementing an authentication service that relies on an
    ///     <see cref="IAuthenticationClient" /> implementation.
    /// </summary>
    /// <param name="authenticationClient">An <see cref="IAuthenticationClient" /> instance.</param>
    /// <param name="cache">A <see cref="HybridCache" /> instance.</param>
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
    ///     Retrieves an access token (token-based authentication).
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     Current access token returned by the service, an empty string, or <see langword="null" />.
    /// </returns>
    /// <remarks>An instance of <see cref="HybridCache" /> is utilized to cache the token by its defined lifetime.</remarks>
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
