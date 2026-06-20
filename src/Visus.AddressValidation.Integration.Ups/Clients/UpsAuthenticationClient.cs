namespace Visus.AddressValidation.Integration.Ups.Clients;

using System.Diagnostics.CodeAnalysis;
using Configuration;
using Http.Clients;
using Microsoft.Extensions.Options;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class UpsAuthenticationClient : AbstractBasicAuthenticationClient
{
    private readonly IOptions<UpsServiceOptions> _options;

    public UpsAuthenticationClient(HttpClient httpClient, IOptions<UpsServiceOptions> options)
        : base(httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string Password => _options.Value.ClientSecret;

    protected override Uri TokenUri => new(_options.Value.EndpointUri, "/security/v1/oauth/token");

    protected override string Username => _options.Value.ClientId;

    protected override void AddAdditionalHeaders(HttpRequestMessage request)
    {
        request.Headers.Add("x-merchant-id", _options.Value.AccountNumber);
    }
}
