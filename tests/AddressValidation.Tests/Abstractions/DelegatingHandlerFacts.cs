namespace Visus.AddressValidation.Tests.Abstractions;

public abstract class DelegatingHandlerFacts
{
	protected sealed class TestHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(new HttpResponseMessage());
		}
	}
}
