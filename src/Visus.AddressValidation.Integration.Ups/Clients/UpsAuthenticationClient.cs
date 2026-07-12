namespace Visus.AddressValidation.Integration.Ups.Clients;

using Configuration;
using Http.Clients;
using Microsoft.Extensions.Options;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class UpsAuthenticationClient : AbstractClientCredentialsAuthenticationClient
{
    private readonly IOptions<UpsServiceOptions> _options;

    public UpsAuthenticationClient(HttpClient httpClient, IOptions<UpsServiceOptions> options)
        : base(httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string ClientId => _options.Value.ClientId;

    protected override string ClientSecret => _options.Value.ClientSecret;

    protected override Uri TokenUri => new(_options.Value.EndpointUri, "/security/v1/oauth/token");

    protected override bool UseHttpBasicAuthentication => true;

    protected override void ApplyAdditionalHeaders(HttpRequestMessage request)
    {
        request.Headers.Add("x-merchant-id", _options.Value.AccountNumber);
    }
}
