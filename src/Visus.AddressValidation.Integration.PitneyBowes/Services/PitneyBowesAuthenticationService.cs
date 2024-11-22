namespace Visus.AddressValidation.Integration.PitneyBowes.Services;

using AddressValidation.Abstractions;
using AddressValidation.Services;
using Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

internal sealed class PitneyBowesAuthenticationService(
	IDistributedCache cache,
	IConfiguration configuration,
	PitneyBowesAuthenticationClient authenticationClient)
	: AbstractAuthenticationService<PitneyBowesAuthenticationClient>(authenticationClient, cache)
{
	private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

	protected override string? GenerateCacheKey()
	{
		string? accountNumber = _configuration[Constants.DeveloperIdConfigurationKey];
		if ( string.IsNullOrWhiteSpace(accountNumber) )
		{
			return null;
		}

		if ( !Enum.TryParse(_configuration[Constants.ClientEnvironmentConfigurationKey], out ClientEnvironment clientEnvironment) )
		{
			clientEnvironment = ClientEnvironment.DEVELOPMENT;
		}

		return $"VS_AVE_CACHE_PB_ACCESS_TOKEN_{accountNumber}:{clientEnvironment}";
	}
}
