namespace Visus.AddressValidation.Integration.Google.Http;

using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using AddressValidation.Http;
using AddressValidation.Serialization.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

internal sealed class GoogleAuthenticationClient : IAuthenticationClient
{
    private static readonly Uri AuthenticationUrl = new("https://oauth2.googleapis.com/token");

    private readonly IConfiguration _configuration;

    private readonly HttpClient _httpClient;

    public GoogleAuthenticationClient(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        string? issuer = _configuration[Constants.ServiceAccountEmailConfigurationKey];
        string? privateKey = _configuration[Constants.PrivateKeyConfigurationKey];
        if ( string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(privateKey) )
        {
            throw new InvalidOperationException($"{Constants.ServiceAccountEmailConfigurationKey} and {Constants.PrivateKeyConfigurationKey} are required.");
        }

        DateTimeOffset currentDateTimeOffset = DateTimeOffset.UtcNow;

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Iat, currentDateTimeOffset.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Exp, currentDateTimeOffset.AddMinutes(60).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new("scope", "https://www.googleapis.com/auth/cloud-platform")
        ];

        string[] privateKeyBlocks = privateKey.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if ( !string.Equals(privateKeyBlocks[0], "BEGIN PRIVATE KEY", StringComparison.OrdinalIgnoreCase) )
        {
            throw new InvalidOperationException($"{Constants.PrivateKeyConfigurationKey} is not a valid PKCS#8 PEM value.");
        }

        using RSA rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBlocks[1]), out _);

        SigningCredentials credentials = new(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            }
        };

        JwtSecurityToken token = new(issuer,
                                     AuthenticationUrl.ToString(),
                                     claims,
                                     signingCredentials: credentials);

        string? jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        if ( string.IsNullOrWhiteSpace(jwtToken) )
        {
            return null;
        }

        List<KeyValuePair<string, string>> payload =
        [
            new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
            new("assertion", jwtToken)
        ];

        using HttpRequestMessage request = new(HttpMethod.Post, AuthenticationUrl);

        request.Content = new FormUrlEncodedContent(payload);

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken)
                                                              .ConfigureAwait(false);

        if ( !response.IsSuccessStatusCode )
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync(DefaultJsonSerializerContext.Default.TokenResponse,
                                                        cancellationToken)
                             .ConfigureAwait(false);
    }
}
