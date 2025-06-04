namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Services;
using Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

internal sealed class GoogleAuthenticationService(
    IDistributedCache cache,
    IConfiguration configuration,
    GoogleAuthenticationClient authenticationClient)
    : AbstractAuthenticationService<GoogleAuthenticationClient>(authenticationClient, cache)
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    protected override string? GenerateCacheKey()
    {
        string? projectId = _configuration[Constants.ProjectIdConfigurationKey];

        return string.IsNullOrWhiteSpace(projectId)
                   ? null
                   : $"VS_AVE_CACHE_GOOGLE_ACCESS_TOKEN_{projectId}";
    }
}
