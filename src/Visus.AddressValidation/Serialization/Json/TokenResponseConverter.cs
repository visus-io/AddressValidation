namespace Visus.AddressValidation.Serialization.Json;

using System.Collections.Frozen;
using System.Text.Json;
using System.Text.Json.Serialization;
using Http;

/// <summary>
///     Converts a <see cref="TokenResponse" /> object to and from json
/// </summary>
public sealed class TokenResponseConverter : JsonConverter<TokenResponse>
{
    private const string AccessTokenPropertyName = "access_token";

    private const string ErrorDescriptionPropertyName = "error_description";

    private const string ExpiresInPropertyName = "expires_in";

    private const string IdentityTokenPropertyName = "identity_token";

    private const string IssuedTokenTypePropertyName = "issued_token_type";

    private const string RefreshTokenPropertyName = "refresh_token";

    private const string TokenTypePropertyName = "token_type";

    private readonly FrozenDictionary<string, string> _propertyMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { AccessTokenPropertyName, nameof(TokenResponse.AccessToken) },
        { ErrorDescriptionPropertyName, nameof(TokenResponse.ErrorDescription) },
        { ExpiresInPropertyName, nameof(TokenResponse.ExpiresIn) },
        { IdentityTokenPropertyName, nameof(TokenResponse.IdentityToken) },
        { IssuedTokenTypePropertyName, nameof(TokenResponse.IssuedTokenType) },
        { RefreshTokenPropertyName, nameof(TokenResponse.RefreshToken) },
        { TokenTypePropertyName, nameof(TokenResponse.TokenType) },
        { nameof(TokenResponse.AccessToken), nameof(TokenResponse.AccessToken) },
        { nameof(TokenResponse.ErrorDescription), nameof(TokenResponse.ErrorDescription) },
        { nameof(TokenResponse.ExpiresIn), nameof(TokenResponse.ExpiresIn) },
        { nameof(TokenResponse.IdentityToken), nameof(TokenResponse.IdentityToken) },
        { nameof(TokenResponse.IssuedTokenType), nameof(TokenResponse.IssuedTokenType) },
        { nameof(TokenResponse.RefreshToken), nameof(TokenResponse.RefreshToken) },
        { nameof(TokenResponse.TokenType), nameof(TokenResponse.TokenType) },
    }.ToFrozenDictionary();

    /// <inheritdoc />
    public override TokenResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        TokenResponse tokenResponse = new();

        while ( reader.Read() )
        {
            if ( reader.TokenType != JsonTokenType.PropertyName )
            {
                continue;
            }

            string? sourcePropertyName = reader.GetString();
            if ( string.IsNullOrWhiteSpace(sourcePropertyName) )
            {
                continue;
            }

            string destinationPropertyName = _propertyMappings.GetValueOrDefault(sourcePropertyName, sourcePropertyName);
            if ( string.IsNullOrWhiteSpace(destinationPropertyName) )
            {
                continue;
            }

            reader.Read();

            switch ( destinationPropertyName )
            {
                case nameof(TokenResponse.AccessToken):
                    tokenResponse.AccessToken = reader.GetString();
                    break;
                case nameof(TokenResponse.ErrorDescription):
                    tokenResponse.ErrorDescription = reader.GetString();
                    break;
                case nameof(TokenResponse.ExpiresIn):
                {
                    // remark: some implementations may return this as string rather than an integer
                    int expiresIn;
                    if ( reader.TokenType == JsonTokenType.String )
                    {
                        if ( !int.TryParse(reader.GetString(), out expiresIn) )
                        {
                            expiresIn = 0;
                        }
                    }
                    else
                    {
                        expiresIn = reader.GetInt32();
                    }

                    tokenResponse.ExpiresIn = expiresIn;
                }
                    break;
                case nameof(TokenResponse.IdentityToken):
                    tokenResponse.IdentityToken = reader.GetString();
                    break;
                case nameof(TokenResponse.IssuedTokenType):
                    tokenResponse.IssuedTokenType = reader.GetString();
                    break;
                case nameof(TokenResponse.RefreshToken):
                    tokenResponse.RefreshToken = reader.GetString();
                    break;
                case nameof(TokenResponse.TokenType):
                    tokenResponse.TokenType = reader.GetString();
                    break;
            }
        }

        return tokenResponse;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TokenResponse? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if ( value is null )
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        writer.WriteString(AccessTokenPropertyName, value.AccessToken);
        writer.WriteString(ErrorDescriptionPropertyName, value.ErrorDescription);
        writer.WriteNumber(ExpiresInPropertyName, value.ExpiresIn);
        writer.WriteString(IdentityTokenPropertyName, value.IdentityToken);
        writer.WriteString(IssuedTokenTypePropertyName, value.IssuedTokenType);
        writer.WriteString(RefreshTokenPropertyName, value.RefreshToken);
        writer.WriteString(TokenTypePropertyName, value.TokenType);

        writer.WriteEndObject();
    }
}
