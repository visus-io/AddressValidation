namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Services;
using Configuration;
using Http;
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

    protected override string? GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.ProjectId)
                   ? null
                   : $"VS_AVE_CACHE_GOOGLE_ACCESS_TOKEN_{_options.Value.ProjectId}";
    }
}
