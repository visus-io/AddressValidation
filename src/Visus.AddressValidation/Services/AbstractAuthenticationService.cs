namespace Visus.AddressValidation.Services;

using System.Diagnostics;
using Diagnostics;
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

    private const string s_activityName = "address_validation.token_fetch";

    private const string s_cacheResultHit = "hit";

    private const string s_cacheResultMiss = "miss";

    private const string s_resultEmptyToken = "empty_token";

    private const string s_resultError = "error";

    private const string s_resultSuccess = "success";

    private const string s_tagCacheResult = "address_validation.cache_result";

    private const string s_tagClientType = "address_validation.client_type";

    private const string s_tagResult = "address_validation.result";

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
                                                            fetched = await FetchTokenAsync(ct).ConfigureAwait(false);
                                                            return fetched;
                                                        },
                                                        null,
                                                        null,
                                                        cancellationToken)
                                                   .ConfigureAwait(false);

        if ( !factoryRan ) // NOSONAR S2583 - false positive: factoryRan is mutated inside the async lambda above
        {
            AddressValidationDiagnostics.CacheResultCounter.Add(
                1,
                new KeyValuePair<string, object?>(s_tagClientType, typeof(TClient).Name),
                new KeyValuePair<string, object?>(s_tagCacheResult, s_cacheResultHit));

            return string.IsNullOrWhiteSpace(tokenResponse?.AccessToken) ? null : tokenResponse.AccessToken;
        }

        AddressValidationDiagnostics.CacheResultCounter.Add(
            1,
            new KeyValuePair<string, object?>(s_tagClientType, typeof(TClient).Name),
            new KeyValuePair<string, object?>(s_tagCacheResult, s_cacheResultMiss));

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

    private async Task<TokenResponse?> FetchTokenAsync(CancellationToken cancellationToken)
    {
        using Activity? activity = AddressValidationDiagnostics.ActivitySource.StartActivity(s_activityName);
        activity?.SetTag(s_tagClientType, typeof(TClient).Name);

        long startTimestamp = Stopwatch.GetTimestamp();
        string result = s_resultSuccess;

        try
        {
            TokenResponse? tokenResponse = await _authenticationClient.RequestClientCredentialsTokenAsync(cancellationToken).ConfigureAwait(false);
            result = tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken) ? s_resultEmptyToken : s_resultSuccess;
            return tokenResponse;
        }
        catch ( Exception ex )
        {
            result = s_resultError;
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
        finally
        {
            activity?.SetTag(s_tagResult, result);
            AddressValidationDiagnostics.TokenFetchDuration.Record(
                Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds,
                new KeyValuePair<string, object?>(s_tagClientType, typeof(TClient).Name),
                new KeyValuePair<string, object?>(s_tagResult, result));
        }
    }
}
