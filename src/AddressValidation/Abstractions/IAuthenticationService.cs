namespace Visus.AddressValidation.Abstractions;

/// <summary>
///     Abstraction for implementing authentication against a web service
/// </summary>
public interface IAuthenticationService
{
	/// <summary>
	///     Retrieves an access token (token-based authentication).
	/// </summary>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
	/// <returns>
	///     Current access token returned by the service, an empty string, or <see langword="null" /> (
	///     <see langword="Nothing" /> in Visual Basic).
	/// </returns>
	/// <remarks>Caching mechanisms should be implemented to avoid superfluous calls.</remarks>
	ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
