---
title: Authentication Client | Custom Integration
uid: custom-authentication-client
---

## Authentication Client

Generally building an integration for **AddressValidation** entails some form of authentication. The default authentication pathway is assumed to be [OAuth2](https://oauth.net/2/).

There are general purpose libraries built to facilitate in this level of authentication such as [IdentityModel](https://github.com/IdentityModel/IdentityModel).
However, to provide maximum performance, reduce third-party dependencies, and support features such as [trimming](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming) and [native AOT deployments](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/), it is
best to use native components with .NET itself.

An authentication client implementation is simply a [typed](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#how-to-use-typed-clients-with-ihttpclientfactory) `HttpClient` who's only responsibility is to request an access token from the service. 

> [!NOTE]
> The [Authentication Service](xref:custom-authentication-service) will handle caching as well as refreshing of the [access token](https://oauth.net/2/access-tokens/).

Below is an example of a basic authentication client that makes a [`client_credentials`](https://www.oauth.com/oauth2-servers/access-tokens/client-credentials/) request to obtain an [access token](https://oauth.net/2/access-tokens/):

```csharp
internal sealed class MyAuthenticationClient(IConfiguration configuration, HttpClient client)
    : IAuthenticationClient
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    
    public async ValueTask<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        string? clientId = _configuration["AVE_MY_CLIENT_ID"];
        string? clientSecret = _configuration["AVE_MY_CLIENT_SECRET"];
        
        Uri requestUri = new("https://example.net/oauth/token");
        
        List<KeyValuePair<string, string>> payload =
        [
            new("grant_type", "client_credentials")
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

> [!IMPORTANT]
> The client **must** implement the [`IAuthenticationClient`](xref:Visus.AddressValidation.Http.IAuthenticationClient) interface.

> [!NOTE]
> The [`DefaultJsonSerializerContext`](xref:Visus.AddressValidation.Serialization.Json.DefaultJsonSerializerContext) instance is used by [`ReadFromJsonAsync(...)`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.httpcontentjsonextensions.readfromjsonasync) method 
> as it implements a System.Text.Json [source generator](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation) for performance.

> [!NOTE]
> It is not necessary for the authentication client to be `internal` but it is **strongly** recommended if redistributing as a library.