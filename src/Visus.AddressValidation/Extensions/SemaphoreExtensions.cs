namespace Visus.AddressValidation.Extensions;

using Abstractions;

internal static class SemaphoreExtensions
{
	internal static async ValueTask<ReleaseToken> LockAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(semaphore);

		await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
		return new ReleaseToken(semaphore);
	}
}
