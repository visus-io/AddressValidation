namespace Visus.AddressValidation.Integration.PitneyBowes.Clients;

using System.Diagnostics.CodeAnalysis;
using Configuration;
using Http.Clients;
using Microsoft.Extensions.Options;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class PitneyBowesAuthenticationClient : AbstractBasicAuthenticationClient
{
    private readonly IOptions<PitneyBowesServiceOptions> _options;

    public PitneyBowesAuthenticationClient(HttpClient httpClient, IOptions<PitneyBowesServiceOptions> options)
        : base(httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string Password => _options.Value.ApiSecret;

    protected override Uri TokenUri => new(_options.Value.EndpointUri, "/oauth/token");

    protected override string Username => _options.Value.ApiKey;
}
