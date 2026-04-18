namespace Visus.AddressValidation.Integration.Ups.Services;

using AddressValidation.Services;
using Configuration;
using Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

internal sealed class UpsAuthenticationService : AbstractAuthenticationService<UpsAuthenticationClient>
{
    private readonly IOptions<UpsServiceOptions> _options;

    public UpsAuthenticationService(HybridCache cache,
                                    IOptions<UpsServiceOptions> options,
                                    UpsAuthenticationClient authenticationClient)
        : base(authenticationClient, cache)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string? GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.AccountNumber)
                   ? null
                   : $"VS_AVE_CACHE_UPS_ACCESS_TOKEN_{_options.Value.AccountNumber}:{_options.Value.ClientEnvironment}";
    }
}
