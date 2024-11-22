namespace Visus.AddressValidation.Http;

using System.Net.Http.Headers;
using System.Text;

/// <summary>
///     HTTP Basic Authentication Authorization Header
/// </summary>
/// <seealso cref="AuthenticationHeaderValue" />
/// <remarks>
///     Instantiates a new instance of the <see cref="BasicAuthenticationHeaderValue" /> class.
/// </remarks>
/// <param name="userName">The name of the user.</param>
/// <param name="password">The password for the given user.</param>
public sealed class BasicAuthenticationHeaderValue(string userName, string? password)
	: AuthenticationHeaderValue("Basic", EncodeCredentials(userName, password))
{
	private static string EncodeCredentials(string username, string? password)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(username);

		if ( string.IsNullOrWhiteSpace(password) )
		{
			password = string.Empty;
		}

		string credential = $"{username}:{password}";

		return Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));
	}
}
