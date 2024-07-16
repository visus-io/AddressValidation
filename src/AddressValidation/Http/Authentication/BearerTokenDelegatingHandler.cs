namespace Visus.AddressValidation.Http.Authentication;

using System.Net.Http.Headers;
using System.Security.Authentication;
using AddressValidation.Abstractions;

/// <summary>
///     An HTTP delegate handler that injects a Bearer token into each request.
/// </summary>
/// <param name="authenticationService">
///     An instance of <see cref="IAuthenticationService" /> that retrieves the
///     access_token.
/// </param>
public sealed class BearerTokenDelegatingHandler(IAuthenticationService authenticationService) : DelegatingHandler
{
	private readonly IAuthenticationService _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

	/// <inheritdoc />
	/// <exception cref="InvalidCredentialException">Provided credentials were rejected by the server.</exception>
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		return SendInternalAsync(request, cancellationToken);
	}

	private async Task<HttpResponseMessage> SendInternalAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if ( request.RequestUri is null )
		{
			return await base.SendAsync(request, cancellationToken);
		}

		string? accessToken = await _authenticationService.GetAccessTokenAsync(cancellationToken);
		if ( string.IsNullOrWhiteSpace(accessToken) )
		{
			throw new InvalidCredentialException();
		}

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		return await base.SendAsync(request, cancellationToken);
	}
}
