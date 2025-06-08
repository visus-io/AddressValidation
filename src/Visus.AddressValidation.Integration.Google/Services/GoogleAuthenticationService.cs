namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Services;
using Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

internal sealed class GoogleAuthenticationService : AbstractAuthenticationService<GoogleAuthenticationClient>
{
    private readonly IConfiguration _configuration;

    public GoogleAuthenticationService(IDistributedCache cache,
                                       IConfiguration configuration,
                                       GoogleAuthenticationClient authenticationClient) 
        : base(authenticationClient, cache)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override string? GenerateCacheKey()
    {
        string? projectId = _configuration[Constants.ProjectIdConfigurationKey];

        return string.IsNullOrWhiteSpace(projectId)
                   ? null
                   : $"VS_AVE_CACHE_GOOGLE_ACCESS_TOKEN_{projectId}";
    }
}
