namespace Visus.AddressValidation.Integration.FedEx.Services;

using AddressValidation.Services;
using Configuration;
using Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

internal sealed class FedExAuthenticationService : AbstractAuthenticationService<FedExAuthenticationClient>
{
    private readonly IOptions<FedExServiceOptions> _options;

    public FedExAuthenticationService(HybridCache cache,
                                      IOptions<FedExServiceOptions> options,
                                      FedExAuthenticationClient authenticationClient)
        : base(authenticationClient, cache)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string? GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.AccountNumber)
                   ? null
                   : $"VS_AVE_CACHE_FDX_ACCESS_TOKEN_{_options.Value.AccountNumber}:{_options.Value.ClientEnvironment}";
    }
}
