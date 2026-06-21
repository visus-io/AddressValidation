---
title: Authentication Service | Custom Integration
uid: custom-authentication-service
---

## Authentication Service

After implementing an [authentication client](xref:custom-authentication-client) it is necessary to implement an authentication service. 

The authentication service wraps around the [authentication client](xref:custom-authentication-client) and ensures the following:

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

Below is an example of a simple authentication service that wraps the [authentication client](xref:custom-authentication-client).

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
> [`GenerateCacheKey()`](xref:Visus.AddressValidation.Services.AbstractAuthenticationService`1#Visus_AddressValidation_Services_AbstractAuthenticationService_1_GenerateCacheKey) **must** return a non-null, non-empty string. The key may only contain letters, digits, underscores, hyphens, and colons. If the required credentials are absent, throw an `InvalidOperationException` rather than returning an empty or invalid value — the base class will throw anyway if the key fails validation.

> [!IMPORTANT]
> The service **must** inherit the [`AbstractAuthenticationService<TClient>`](xref:Visus.AddressValidation.Services.AbstractAuthenticationService`1) class.

> [!NOTE]
> [`BearerTokenDelegatingHandler<TClient>`](xref:Visus.AddressValidation.Http.BearerTokenDelegatingHandler`1) leverages this service to automatically attach a [bearer token](https://oauth.net/2/bearer-tokens/) to the request. Refer to the <xref:custom-registering-services> page for details.

> [!NOTE]
> It is not necessary for the authentication service to be `internal` but it is **strongly** recommended if redistributing as a library.