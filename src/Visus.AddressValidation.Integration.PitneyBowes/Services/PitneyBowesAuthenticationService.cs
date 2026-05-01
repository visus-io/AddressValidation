namespace Visus.AddressValidation.Integration.PitneyBowes.Services;

using AddressValidation.Services;
using Clients;
using Configuration;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

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

    protected override string? GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.DeveloperId)
                   ? null
                   : $"VS_AVE_CACHE_PB_ACCESS_TOKEN_{_options.Value.DeveloperId}:{_options.Value.ClientEnvironment}";
    }
}
