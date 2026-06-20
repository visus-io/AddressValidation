namespace Visus.AddressValidation.Integration.Ups.Services;

using AddressValidation.Services;
using Clients;
using Configuration;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
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

    protected override string GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.AccountNumber)
                   ? throw new InvalidOperationException($"{nameof(UpsServiceOptions.AccountNumber)} is required to generate a cache key.")
                   : $"{CacheKeyTag}ups:{_options.Value.AccountNumber}:{_options.Value.ClientEnvironment}";
    }
}
