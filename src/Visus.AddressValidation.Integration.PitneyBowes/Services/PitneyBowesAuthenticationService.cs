namespace Visus.AddressValidation.Integration.PitneyBowes.Services;

using System.Diagnostics.CodeAnalysis;
using AddressValidation.Services;
using Clients;
using Configuration;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class PitneyBowesAuthenticationService : AbstractAuthenticationService<PitneyBowesAuthenticationClient>
{
    private readonly IOptions<PitneyBowesServiceOptions> _options;

    public PitneyBowesAuthenticationService(HybridCache cache,
                                            IOptions<PitneyBowesServiceOptions> options,
                                            PitneyBowesAuthenticationClient authenticationClient)
        : base(authenticationClient, cache)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.DeveloperId)
                   ? throw new InvalidOperationException($"{nameof(PitneyBowesServiceOptions.DeveloperId)} is required to generate a cache key.")
                   : $"{CacheKeyTag}pb:{_options.Value.DeveloperId}:{_options.Value.ClientEnvironment}";
    }
}
