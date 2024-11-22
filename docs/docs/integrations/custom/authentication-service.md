---
title: Authentication Service | Custom Integration
uid: custom-authentication-service
---

## Authentication Service

After implementing an [authentication client](xref:custom-authentication-client) it is necessary to implement an authentication service. 

The authentication service wraps around the [authentication client](xref:custom-authentication-client) and ensures the following:

- An authentication request is made to the underlying service for an [access token](https://oauth.net/2/access-tokens/).
- The access token that is cached by an [`IDistributedCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache) instance to avoid superfluous calls.
  - If the cache hit misses due to expiration, an authentication request is made to retrieve a new token.

Below is an example of a simple authentication service that wraps the [authentication client](xref:custom-authentication-client).

```csharp
internal sealed class MyAuthenticationService(
    IDistributedCache cache,
    IConfiguration configuration,
    MyAuthenticationClient authenticationClient)
    : AbstractAuthenticationService<MyAuthenticationClient>(authenticationClient, cache)
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    
    protected override string? GenerateCacheKey()
    {
        string? uniqueIdentifier = _configuration["AVE_MY_UNIQUE_IDENTIFIER"];
        if ( string.IsNullOrWhiteSpace(uniqueIdentifier) )
        {
            return null;
        }
        
        return $"AVE_CACHE_MY_ACCESS_TOKEN_{uniqueIdentifier}";
    }
}
```

> [!IMPORTANT]
> [`GenerateCacheKey()`](xref:Visus.AddressValidation.Services.AbstractAuthenticationService`1#Visus_AddressValidation_Services_AbstractAuthenticationService_1_GenerateCacheKey) **must** return a static unique value. If no value is returned, the [authentication client](xref:custom-authentication-client) **will not be called**.

> [!IMPORTANT]
> The service **must** inherit the [`AbstractAuthenticationService<TClient>`](xref:Visus.AddressValidation.Services.AbstractAuthenticationService`1) class.

> [!NOTE]
> [`BearerTokenDelegatingHandler<TClient>`](xref:Visus.AddressValidation.Http.BearerTokenDelegatingHandler`1) leverages this service to automatically attach a [bearer token](https://oauth.net/2/bearer-tokens/) to the request. Refer to the <xref:custom-registering-services> page for details.

> [!NOTE]
> It is not necessary for the authentication service to be `internal` but it is **strongly** recommended if redistributing as a library.