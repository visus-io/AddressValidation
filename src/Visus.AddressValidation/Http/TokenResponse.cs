namespace Visus.AddressValidation.Http;

using System.Text.Json.Serialization;
using Serialization.Json;

/// <summary>
///     Represents a response from an OpenID Connect/OAuth2 token endpoint
/// </summary>
[JsonConverter(typeof(TokenResponseConverter))]
public sealed class TokenResponse
{
	/// <summary>
	///     Gets the access token
	/// </summary>
	public string? AccessToken { get; set; }

	/// <summary>
	///     Gets the error description
	/// </summary>
	public string? ErrorDescription { get; set; }

	/// <summary>
	///     Gets the expires in
	/// </summary>
	public int ExpiresIn { get; set; }

	/// <summary>
	///     Gets the identity token
	/// </summary>
	public string? IdentityToken { get; set; }

	/// <summary>
	///     Gets the issued token type
	/// </summary>
	public string? IssuedTokenType { get; set; }

	/// <summary>
	///     Gets the refresh token
	/// </summary>
	public string? RefreshToken { get; set; }

	/// <summary>
	///     Gets the scope
	/// </summary>
	public string? Scope { get; set; }

	/// <summary>
	///     Gets the token type
	/// </summary>
	public string? TokenType { get; set; }
}
