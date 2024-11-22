namespace Visus.AddressValidation.Abstractions;

internal readonly struct ReleaseToken(SemaphoreSlim semaphore) : IDisposable
{
	private readonly SemaphoreSlim _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore));

	public void Dispose()
	{
		_semaphore.Release();
	}
}
