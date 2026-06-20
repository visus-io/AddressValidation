namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Services;
using Clients;
using Configuration;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

internal sealed class GoogleAuthenticationService : AbstractAuthenticationService<GoogleAuthenticationClient>
{
    private readonly IOptions<GoogleServiceOptions> _options;

    public GoogleAuthenticationService(HybridCache cache,
                                       IOptions<GoogleServiceOptions> options,
                                       GoogleAuthenticationClient authenticationClient)
        : base(authenticationClient, cache)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.ProjectId)
                   ? throw new InvalidOperationException($"{nameof(GoogleServiceOptions.ProjectId)} is required to generate a cache key.")
                   : $"{CacheKeyTag}google:{_options.Value.ProjectId}";
    }
}
