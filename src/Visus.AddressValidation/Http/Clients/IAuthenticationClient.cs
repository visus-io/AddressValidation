namespace Visus.AddressValidation.Http.Clients;

/// <summary>
///     Defines the contract for an OAuth 2.0 client-credentials authentication client.
/// </summary>
public interface IAuthenticationClient
{
    /// <summary>
    ///     Sends a token request using the client_credentials grant type.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     An instance of <see cref="TokenResponse" /> containing the token or <see langword="null" />.
    /// </returns>
    Task<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default);
}
