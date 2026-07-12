namespace Visus.AddressValidation.Integration.FedEx.Clients;

using Configuration;
using Http.Clients;
using Microsoft.Extensions.Options;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class FedExAuthenticationClient : AbstractClientCredentialsAuthenticationClient
{
    private readonly IOptions<FedExServiceOptions> _options;

    public FedExAuthenticationClient(HttpClient httpClient, IOptions<FedExServiceOptions> options)
        : base(httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string ClientId => _options.Value.ClientId;

    protected override string ClientSecret => _options.Value.ClientSecret;

    protected override Uri TokenUri => new(_options.Value.EndpointUri, "/oauth/token");
}
