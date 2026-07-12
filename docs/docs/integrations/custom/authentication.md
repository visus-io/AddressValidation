---
title: Authentication | Custom Integration
uid: custom-authentication
---

## Authentication Client

An authentication client is a [typed](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#how-to-use-typed-clients-with-ihttpclientfactory) `HttpClient` whose sole responsibility is requesting an access token from the provider. The [Authentication Service](#authentication-service) handles caching and refresh. The client only needs to make the token request.

> [!NOTE]
> There are general-purpose libraries for OAuth 2.0 such as [IdentityModel](https://github.com/IdentityModel/IdentityModel). To maintain maximum performance, avoid third-party dependencies, and support [trimming](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming) and [native AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/), it is best to use native .NET components instead.

### Using AbstractBasicAuthenticationClient

For providers that use the [`client_credentials`](https://www.oauth.com/oauth2-servers/access-tokens/client-credentials/) grant with [HTTP Basic Authentication](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Authentication#basic_authentication_scheme), extend [`AbstractBasicAuthenticationClient`](xref:Visus.AddressValidation.Http.Clients.AbstractBasicAuthenticationClient). The base class handles the token request, Basic Auth header, and JSON deserialization; you only implement three abstract properties:

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class MyAuthenticationClient : AbstractBasicAuthenticationClient
{
    private readonly IOptions<MyServiceOptions> _options;

    public MyAuthenticationClient(HttpClient httpClient, IOptions<MyServiceOptions> options)
        : base(httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string Username => _options.Value.ClientId;

    protected override string Password => _options.Value.ClientSecret;

    protected override Uri TokenUri => new(_options.Value.EndpointUri, "/oauth/token");
}
```

If the provider requires additional headers on the token request, override the virtual `AddAdditionalHeaders` method:

```csharp
protected override void AddAdditionalHeaders(HttpRequestMessage request)
{
    request.Headers.Add("x-merchant-id", _options.Value.AccountNumber);
}
```

### Custom flows

For providers that use a non-standard grant type, bearer token in the request body, or another scheme that does not fit the Basic Auth pattern, implement [`IAuthenticationClient`](xref:Visus.AddressValidation.Http.Clients.IAuthenticationClient) directly:

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class MyAuthenticationClient : IAuthenticationClient
{
    private readonly HttpClient _httpClient;

    private readonly IOptions<MyServiceOptions> _options;

    internal MyAuthenticationClient(HttpClient httpClient, IOptions<MyServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        Uri requestUri = new(_options.Value.EndpointUri, "/oauth/token");

        List<KeyValuePair<string, string>> payload =
        [
            new("grant_type", "client_credentials"),
            new("client_id", _options.Value.ClientId),
            new("client_secret", _options.Value.ClientSecret),
        ];

        using HttpRequestMessage request = new(HttpMethod.Post, requestUri);

        request.Content = new FormUrlEncodedContent(payload);

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if ( !response.IsSuccessStatusCode )
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync(DefaultJsonSerializerContext.Default.TokenResponse, cancellationToken)
                                     .ConfigureAwait(false);
    }
}
```

> [!NOTE]
> The [`DefaultJsonSerializerContext`](xref:Visus.AddressValidation.Serialization.Json.DefaultJsonSerializerContext) instance passed to [`ReadFromJsonAsync(...)`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.httpcontentjsonextensions.readfromjsonasync) is a System.Text.Json [source generator](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation), required for correct behavior in trimmed and native AOT deployments.

> [!NOTE]
> It is not necessary for the authentication client to be `internal` but it is **strongly** recommended if redistributing as a library.

## Authentication Service

The authentication service wraps around the [authentication client](#authentication-client) and ensures the following:

- An authentication request is made to the underlying service for an [access token](https://oauth.net/2/access-tokens/).
- The access token that is cached by [`HybridCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.hybrid.hybridcache) to avoid superfluous calls.
  - If the cache hit misses due to expiration, an authentication request is made to retrieve a new token.

The example below assumes a `MyServiceOptions` class that extends [`AbstractServiceOptions`](xref:Visus.AddressValidation.Configuration.AbstractServiceOptions) and exposes the provider credentials:

```csharp
public sealed class MyServiceOptions : AbstractServiceOptions
{
    public const string SectionName = "AddressValidationSettings:MyProvider";

    public override Uri EndpointUri => /* ... */;

    [Required(AllowEmptyStrings = false)]
    public required string ClientId { get; set; }

    [Required(AllowEmptyStrings = false)]
    public required string ClientSecret { get; set; }
}
```

> [!NOTE]
> See [Registering Services](xref:custom-registering-services) for the complete options class definition, including the `EndpointUri` implementation and source-generated validator.

Below is an example of a simple authentication service that wraps the [authentication client](#authentication-client).

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class MyAuthenticationService : AbstractAuthenticationService<MyAuthenticationClient>
{
    private readonly IOptions<MyServiceOptions> _options;

    internal MyAuthenticationService(
        HybridCache cache,
        IOptions<MyServiceOptions> options,
        MyAuthenticationClient authenticationClient)
        : base(authenticationClient, cache)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override string GenerateCacheKey()
    {
        return string.IsNullOrWhiteSpace(_options.Value.ClientId)
                   ? throw new InvalidOperationException($"{nameof(MyServiceOptions.ClientId)} is required to generate a cache key.")
                   : $"{CacheKeyTag}my-provider:{_options.Value.ClientId}:{_options.Value.ClientEnvironment}";
    }
}
```

> [!IMPORTANT]
> [`GenerateCacheKey()`](xref:Visus.AddressValidation.Services.AbstractAuthenticationService`1#Visus_AddressValidation_Services_AbstractAuthenticationService_1_GenerateCacheKey) **must** return a non-null, non-empty string. The key may only contain letters, digits, underscores, hyphens, and colons. If the required credentials are absent, throw an `InvalidOperationException` rather than returning an empty or invalid value. The base class will throw anyway if the key fails validation.

> [!IMPORTANT]
> The service **must** inherit the [`AbstractAuthenticationService<TClient>`](xref:Visus.AddressValidation.Services.AbstractAuthenticationService`1) class.

> [!NOTE]
> [`BearerTokenDelegatingHandler<TClient>`](xref:Visus.AddressValidation.Http.BearerTokenDelegatingHandler`1) uses this service to automatically attach a [bearer token](https://oauth.net/2/bearer-tokens/) to the request. Refer to the <xref:custom-registering-services> page for details.

> [!NOTE]
> It is not necessary for the authentication service to be `internal` but it is **strongly** recommended if redistributing as a library.
