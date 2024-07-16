namespace AddressValidation.Tests.Http.Authentication;

using System.Web;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Http.Authentication;
using Moq;

public sealed class BearerTokenDelegatingHandlerFacts : DelegatingHandlerFacts
{
	[Fact]
	public async Task SendAsync_Success()
	{
		const string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

		var authenticationServiceMock = new Mock<IAuthenticationService>();

		authenticationServiceMock.Setup(s => s.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
								 .ReturnsAsync(() => accessToken);
		
		var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
		var handler = new BearerTokenDelegatingHandler(authenticationServiceMock.Object)
		{
			InnerHandler = new TestHandler()
		};

		var invoker = new HttpMessageInvoker(handler);
		_ = await invoker.SendAsync(httpRequestMessage, default);

		Assert.NotNull(httpRequestMessage.Headers.Authorization);
		Assert.Contains(accessToken, httpRequestMessage.Headers.Authorization.ToString());
	}
}
