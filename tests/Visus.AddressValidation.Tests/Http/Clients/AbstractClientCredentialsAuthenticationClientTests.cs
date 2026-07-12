namespace Visus.AddressValidation.Tests.Http.Clients;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AddressValidation.Http;
using AddressValidation.Http.Clients;

internal sealed class AbstractClientCredentialsAuthenticationClientTests
{
    private static readonly Uri s_tokenUri = new("https://example.test/oauth/token");

    [Test]
    public async Task RequestClientCredentialsTokenAsync_WhenResponseIsNotSuccessStatusCode_ThrowsHttpRequestException(CancellationToken cancellationToken)
    {
        using StubHttpMessageHandler handler = new(HttpStatusCode.Unauthorized, string.Empty);
        using HttpClient httpClient = new(handler);
        TestAuthenticationClient sut = new(httpClient, s_tokenUri, "client-id", "client-secret");

        Func<Task> act = () => sut.RequestClientCredentialsTokenAsync(cancellationToken);

        await act.Should().ThrowExactlyAsync<HttpRequestException>().ConfigureAwait(false);
    }

    [Test]
    public async Task RequestClientCredentialsTokenAsync_WithApplyAdditionalHeadersOverride_InvokesHookBeforeSending(CancellationToken cancellationToken)
    {
        using StubHttpMessageHandler handler = new(HttpStatusCode.OK, """{"access_token":"test-token","expires_in":3600,"token_type":"Bearer"}""");
        using HttpClient httpClient = new(handler);
        TestAuthenticationClient sut = new(
            httpClient,
            s_tokenUri,
            "client-id",
            "client-secret",
            addAdditionalHeaders: request => request.Headers.Add("x-custom", "custom-value"));

        await sut.RequestClientCredentialsTokenAsync(cancellationToken).ConfigureAwait(false);

        handler.CapturedHeaders.Should().NotBeNull();
        handler.CapturedHeaders!.GetValues("x-custom").Should().Equal("custom-value");
    }

    [Test]
    public async Task RequestClientCredentialsTokenAsync_WithDefaultHooks_SendsClientCredentialsInRequestBodyWithNoAuthorizationHeader(CancellationToken cancellationToken)
    {
        using StubHttpMessageHandler handler = new(HttpStatusCode.OK, """{"access_token":"test-token","expires_in":3600,"token_type":"Bearer"}""");
        using HttpClient httpClient = new(handler);
        TestAuthenticationClient sut = new(httpClient, s_tokenUri, "client-id", "client-secret");

        TokenResponse? result = await sut.RequestClientCredentialsTokenAsync(cancellationToken).ConfigureAwait(false);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("test-token");
        handler.CapturedRequestUri.Should().Be(s_tokenUri);
        handler.CapturedAuthorization.Should().BeNull();
        handler.CapturedRequestBody.Should().Be("grant_type=client_credentials&client_id=client-id&client_secret=client-secret");
    }

    [Test]
    public async Task RequestClientCredentialsTokenAsync_WithHttpBasicAuthenticationEnabled_SendsBasicAuthorizationHeaderWithGrantTypeOnlyBody(CancellationToken cancellationToken)
    {
        using StubHttpMessageHandler handler = new(HttpStatusCode.OK, """{"access_token":"test-token","expires_in":3600,"token_type":"Bearer"}""");
        using HttpClient httpClient = new(handler);
        TestAuthenticationClient sut = new(httpClient, s_tokenUri, "client-id", "client-secret", true);

        await sut.RequestClientCredentialsTokenAsync(cancellationToken).ConfigureAwait(false);

        handler.CapturedAuthorization.Should().NotBeNull();
        handler.CapturedAuthorization!.Scheme.Should().Be("Basic");
        handler.CapturedAuthorization.Parameter.Should().Be(Convert.ToBase64String("client-id:client-secret"u8.ToArray()));
        handler.CapturedRequestBody.Should().Be("grant_type=client_credentials");
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseBody;

        private readonly HttpStatusCode _statusCode;

        public StubHttpMessageHandler(HttpStatusCode statusCode, string responseBody)
        {
            _statusCode = statusCode;
            _responseBody = responseBody;
        }

        public AuthenticationHeaderValue? CapturedAuthorization { get; private set; }

        public HttpRequestHeaders? CapturedHeaders { get; private set; }

        public string? CapturedRequestBody { get; private set; }

        public Uri? CapturedRequestUri { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CapturedAuthorization = request.Headers.Authorization;
            CapturedHeaders = request.Headers;
            CapturedRequestUri = request.RequestUri;
            CapturedRequestBody = request.Content is null
                                      ? null
                                      : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            return new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseBody, Encoding.UTF8, "application/json"),
            };
        }
    }

    private sealed class TestAuthenticationClient : AbstractClientCredentialsAuthenticationClient
    {
        private readonly Action<HttpRequestMessage>? _addAdditionalHeaders;

        public TestAuthenticationClient(
            HttpClient httpClient,
            Uri tokenUri,
            string clientId,
            string clientSecret,
            bool useHttpBasicAuthentication = false,
            Action<HttpRequestMessage>? addAdditionalHeaders = null)
            : base(httpClient)
        {
            TokenUri = tokenUri;
            ClientId = clientId;
            ClientSecret = clientSecret;
            UseHttpBasicAuthentication = useHttpBasicAuthentication;
            _addAdditionalHeaders = addAdditionalHeaders;
        }

        protected override string ClientId { get; }

        protected override string ClientSecret { get; }

        protected override Uri TokenUri { get; }

        protected override bool UseHttpBasicAuthentication { get; }

        protected override void ApplyAdditionalHeaders(HttpRequestMessage request)
        {
            _addAdditionalHeaders?.Invoke(request);
        }
    }
}
