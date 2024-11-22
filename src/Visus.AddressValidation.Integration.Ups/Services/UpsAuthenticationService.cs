namespace Visus.AddressValidation.Integration.Ups.Services;

using AddressValidation.Abstractions;
using AddressValidation.Services;
using Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

internal sealed class UpsAuthenticationService(
	IDistributedCache cache,
	IConfiguration configuration,
	UpsAuthenticationClient authenticationClient)
	: AbstractAuthenticationService<UpsAuthenticationClient>(authenticationClient, cache)
{
	private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

	protected override string? GenerateCacheKey()
	{
		string? accountNumber = _configuration[Constants.AccountNumberConfigurationKey];
		if ( string.IsNullOrWhiteSpace(accountNumber) )
		{
			return null;
		}

		if ( !Enum.TryParse(_configuration[Constants.ClientEnvironmentConfigurationKey], out ClientEnvironment clientEnvironment) )
		{
			clientEnvironment = ClientEnvironment.DEVELOPMENT;
		}

		return $"VS_AVE_CACHE_UPS_ACCESS_TOKEN_{accountNumber}:{clientEnvironment}";
	}
}
