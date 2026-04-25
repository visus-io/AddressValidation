namespace Visus.AddressValidation.Serialization.Json;

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Http;

/// <summary>
///     Converts a <see cref="TokenResponse" /> object to and from JSON.
/// </summary>
/// <remarks>
///     This converter handles both snake_case JSON property names (e.g. <c>access_token</c>) and
///     PascalCase property names (e.g. <c>AccessToken</c>). The <see cref="TokenResponse.ExpiresIn" />
///     field is handled as either a JSON number or a string representation of an integer.
/// </remarks>
public sealed class TokenResponseConverter : JsonConverter<TokenResponse>
{
    private const string AccessTokenPropertyName = "access_token";

    private const string ErrorDescriptionPropertyName = "error_description";

    private const string ExpiresInPropertyName = "expires_in";

    private const string IdentityTokenPropertyName = "identity_token";

    private const string IssuedTokenTypePropertyName = "issued_token_type";

    private static readonly FrozenDictionary<string, string> PropertyMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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

    private const string RefreshTokenPropertyName = "refresh_token";

    private const string TokenTypePropertyName = "token_type";

    /// <inheritdoc />
    /// <summary>
    ///     Reads and converts JSON into a <see cref="TokenResponse" /> object.
    /// </summary>
    /// <param name="reader">The reader to read JSON from.</param>
    /// <param name="typeToConvert">The type to convert. Must be <see cref="TokenResponse" />.</param>
    /// <param name="options">The serializer options to use during deserialization.</param>
    /// <returns>A <see cref="TokenResponse" /> populated from the JSON data.</returns>
    [SuppressMessage("Design", "MA0051:Method is too long",
        Justification = "Necessary to properly handle all properties in a single pass through the JSON data.")]
    public override TokenResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? accessToken = null;
        string? errorDescription = null;
        int expiresIn = 0;
        string? identityToken = null;
        string? issuedTokenType = null;
        string? refreshToken = null;
        string? scope = null;
        string? tokenType = null;

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

            string destinationPropertyName = PropertyMappings.GetValueOrDefault(sourcePropertyName, sourcePropertyName);
            if ( string.IsNullOrWhiteSpace(destinationPropertyName) )
            {
                continue;
            }

            reader.Read();

            switch ( destinationPropertyName )
            {
                case nameof(TokenResponse.AccessToken):
                    accessToken = reader.GetString();
                    break;
                case nameof(TokenResponse.ErrorDescription):
                    errorDescription = reader.GetString();
                    break;
                case nameof(TokenResponse.ExpiresIn):
                    expiresIn = ParseExpiresIn(ref reader);
                    break;
                case nameof(TokenResponse.IdentityToken):
                    identityToken = reader.GetString();
                    break;
                case nameof(TokenResponse.IssuedTokenType):
                    issuedTokenType = reader.GetString();
                    break;
                case nameof(TokenResponse.RefreshToken):
                    refreshToken = reader.GetString();
                    break;
                case nameof(TokenResponse.Scope):
                    scope = reader.GetString();
                    break;
                case nameof(TokenResponse.TokenType):
                    tokenType = reader.GetString();
                    break;
            }
        }

        return new TokenResponse(
            accessToken,
            refreshToken,
            identityToken,
            issuedTokenType,
            expiresIn,
            scope,
            tokenType,
            errorDescription);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Writes a <see cref="TokenResponse" /> object as JSON.
    /// </summary>
    /// <param name="writer">The writer to write JSON to.</param>
    /// <param name="value">The <see cref="TokenResponse" /> value to serialize. May be <see langword="null" />.</param>
    /// <param name="options">The serializer options to use during serialization.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer" /> is <see langword="null" />.</exception>
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

    /// <summary>
    ///     Parses the <c>expires_in</c> value from the current JSON token, supporting both
    ///     numeric and string representations.
    /// </summary>
    /// <param name="reader">The reader positioned at the value token to parse.</param>
    /// <returns>
    ///     The parsed integer value, or <c>0</c> if the token is a string that cannot be parsed
    ///     as an integer.
    /// </returns>
    private static int ParseExpiresIn(ref Utf8JsonReader reader)
    {
        if ( reader.TokenType == JsonTokenType.String )
        {
            return int.TryParse(reader.GetString(), CultureInfo.InvariantCulture, out int expiresIn) ? expiresIn : 0;
        }

        return reader.GetInt32();
    }
}
