namespace Visus.AddressValidation.Http;

using System.Net.Http.Headers;
using System.Security.Authentication;
using Clients;
using Services;

/// <summary>
///     An HTTP delegate handler that injects a Bearer token into each request.
/// </summary>
public sealed class BearerTokenDelegatingHandler<TClient> : DelegatingHandler
    where TClient : IAuthenticationClient
{
    private readonly AbstractAuthenticationService<TClient> _authenticationService;

    /// <summary>
    ///     Initializes a new instance of <see cref="BearerTokenDelegatingHandler{TClient}" />.
    /// </summary>
    /// <param name="authenticationService">
    ///     An instance of <see cref="AbstractAuthenticationService{T}" /> that retrieves the
    ///     access_token.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="authenticationService" /> is <see langword="null" />.
    /// </exception>
    public BearerTokenDelegatingHandler(AbstractAuthenticationService<TClient> authenticationService)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

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
