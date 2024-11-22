namespace Visus.AddressValidation.Http;

using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using Services;

/// <summary>
///     An HTTP delegate handler that injects a Bearer token into each request.
/// </summary>
/// <param name="authenticationService">
///     An instance of <see cref="AbstractAuthenticationService{T}" /> that retrieves the
///     access_token.
/// </param>
public sealed class BearerTokenDelegatingHandler<TClient>(AbstractAuthenticationService<TClient> authenticationService) : DelegatingHandler
	where TClient : IAuthenticationClient
{
	private readonly AbstractAuthenticationService<TClient> _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

	/// <inheritdoc />
	/// <exception cref="InvalidCredentialException">Provided credentials were rejected by the server.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		return SendInternalAsync(request, cancellationToken);
	}

	private async Task<HttpResponseMessage> SendInternalAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if ( request.RequestUri is null )
		{
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}

		string? accessToken = await _authenticationService.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
		if ( string.IsNullOrWhiteSpace(accessToken) )
		{
			throw new InvalidCredentialException();
		}

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}
}
