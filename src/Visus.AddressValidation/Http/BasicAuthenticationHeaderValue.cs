namespace Visus.AddressValidation.Http;

using System.Net.Http.Headers;
using System.Text;

/// <summary>
///     HTTP Basic Authentication Authorization Header
/// </summary>
/// <seealso cref="AuthenticationHeaderValue" />
public sealed class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
{
    /// <summary>
    ///     Initializes a new instance of <see cref="BasicAuthenticationHeaderValue" />.
    /// </summary>
    /// <param name="userName">The name of the user.</param>
    /// <param name="password">The password for the given user.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="userName" /> is <see langword="null" /> or whitespace.
    /// </exception>
    public BasicAuthenticationHeaderValue(string userName, string? password)
        : base("Basic", EncodeCredentials(userName, password))
    {
    }

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
