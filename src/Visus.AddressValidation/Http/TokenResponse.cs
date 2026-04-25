namespace Visus.AddressValidation.Http;

using System.Text.Json.Serialization;
using Serialization.Json;

/// <summary>
///     Represents a token response returned by an OAuth 2.0 / OpenID Connect token endpoint.
/// </summary>
/// <param name="AccessToken">The access token issued by the authorization server.</param>
/// <param name="RefreshToken">
///     The refresh token, which can be used to obtain new access tokens, or <see langword="null" /> if not issued.
/// </param>
/// <param name="IdentityToken">
///     The identity token containing claims about the authenticated user, or <see langword="null" /> if not issued.
/// </param>
/// <param name="IssuedTokenType">
///     A URI that indicates the type of the issued security token, as defined by RFC 8693, or
///     <see langword="null" /> if not present.
/// </param>
/// <param name="ExpiresIn">The lifetime in seconds of the access token.</param>
/// <param name="Scope">
///     The scope of the access token as a space-delimited list of scope values, or <see langword="null" /> if
///     identical to the requested scope.
/// </param>
/// <param name="TokenType">
///     The type of the token issued (e.g. <c>Bearer</c>), or <see langword="null" /> if not present.
/// </param>
/// <param name="ErrorDescription">
///     A human-readable description of the error that occurred during token issuance, or
///     <see langword="null" /> if the request was successful.
/// </param>
[JsonConverter(typeof(TokenResponseConverter))]
public sealed record TokenResponse(
    string? AccessToken,
    string? RefreshToken,
    string? IdentityToken,
    string? IssuedTokenType,
    int ExpiresIn,
    string? Scope,
    string? TokenType,
    string? ErrorDescription);
