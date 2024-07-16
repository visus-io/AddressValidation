namespace AddressValidation.Tests.Http.Authentication;

using System.Web;
using Abstractions;
using AddressValidation.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Moq;

public sealed class QueryStringDelegatingHandlerFacts : DelegatingHandlerFacts
{
	[Fact]
	public async Task SendAsync_Success()
	{
		const string configurationKey = "API_KEY";
		const string queryKey = "key";

		const string apiKey = "nF4XVxKf3ncwp_8bM2MWWKfNJho-al37bdIVoMs2VJZzjh00QIiz7A";

		var configurationMock = new Mock<IConfiguration>();
		configurationMock.Setup(s => s[configurationKey])
						 .Returns(apiKey);

		var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
		var handler = new QueryStringDelegatingHandler(configurationMock.Object, configurationKey, queryKey)
		{
			InnerHandler = new TestHandler()
		};

		var invoker = new HttpMessageInvoker(handler);
		_ = await invoker.SendAsync(httpRequestMessage, default);

		var query = HttpUtility.ParseQueryString(httpRequestMessage.RequestUri!.Query);

		Assert.NotNull(query[queryKey]);
		Assert.Equal(apiKey, query[queryKey]);
	}
}
