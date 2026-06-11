namespace Visus.AddressValidation.Integration.FedEx.Services;

using AddressValidation.Services;
using Clients;
using Configuration;
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

    protected override string GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.AccountNumber) 
                   ? throw new InvalidOperationException($"{nameof(FedExServiceOptions.AccountNumber)} is required to generate a cache key.") 
                   : $"{CacheKeyTag}fdx:{_options.Value.AccountNumber}:{_options.Value.ClientEnvironment}";
    }
}
