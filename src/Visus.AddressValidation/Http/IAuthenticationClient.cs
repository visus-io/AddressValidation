namespace Visus.AddressValidation.Http;

/// <summary>
///     Abstraction for implementing an authentication client
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
	ValueTask<TokenResponse?> RequestClientCredentialsTokenAsync(CancellationToken cancellationToken = default);
}
