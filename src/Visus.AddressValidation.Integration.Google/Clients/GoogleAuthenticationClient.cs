namespace Visus.AddressValidation.Integration.Google.Clients;

using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using AddressValidation.Serialization.Json;
using Configuration;
using Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

internal sealed class GoogleAuthenticationClient : IAuthenticationClient
{
    private static readonly Uri AuthenticationUrl = new("https://oauth2.googleapis.com/token");

    private readonly HttpClient _httpClient;

    private readonly IOptions<GoogleServiceOptions> _options;

    public GoogleAuthenticationClient(HttpClient httpClient, IOptions<GoogleServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        DateTimeOffset currentDateTimeOffset = DateTimeOffset.UtcNow;

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Iat, currentDateTimeOffset.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Exp, currentDateTimeOffset.AddMinutes(60).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new("scope", "https://www.googleapis.com/auth/cloud-platform"),
        ];

        string[] privateKeyBlocks = _options.Value.PrivateKey.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if ( !string.Equals(privateKeyBlocks[0], "BEGIN PRIVATE KEY", StringComparison.OrdinalIgnoreCase) )
        {
            throw new InvalidOperationException($"{nameof(GoogleServiceOptions.PrivateKey)} is not a valid PKCS#8 PEM value.");
        }

        using RSA rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBlocks[1]), out _);

        SigningCredentials credentials = new(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false,
            },
        };

        JwtSecurityToken token = new(_options.Value.ServiceAccountEmail,
            AuthenticationUrl.ToString(),
            claims,
            signingCredentials: credentials);

        string? jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        if ( string.IsNullOrWhiteSpace(jwtToken) )
        {
            throw new InvalidOperationException("JWT token generation returned an empty or null value.");
        }

        List<KeyValuePair<string, string>> payload =
        [
            new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
            new("assertion", jwtToken),
        ];

        using HttpRequestMessage request = new(HttpMethod.Post, AuthenticationUrl);

        request.Content = new FormUrlEncodedContent(payload);

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken)
                                                              .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(DefaultJsonSerializerContext.Default.TokenResponse,
                                  cancellationToken)
                             .ConfigureAwait(false);
    }
}
