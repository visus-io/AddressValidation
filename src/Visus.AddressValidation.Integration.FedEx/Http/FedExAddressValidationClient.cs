namespace Visus.AddressValidation.Integration.FedEx.Http;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using AddressValidation.Abstractions;
using Configuration;
using Microsoft.Extensions.Options;
using Serialization.Json;

internal sealed class FedExAddressValidationClient
{
    private readonly HttpClient _httpClient;

    private readonly IOptions<FedExServiceOptions> _options;

    public FedExAddressValidationClient(HttpClient httpClient, IOptions<FedExServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<ApiResponse?> ValidateAddressAsync(FedExAddressValidationRequest request,
                                                        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateAddressInternalAsync(request, cancellationToken);
    }

    private async ValueTask<ApiResponse?> ValidateAddressInternalAsync(FedExAddressValidationRequest request,
                                                                       CancellationToken cancellationToken = default)
    {
        Uri baseUri = _options.Value.ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointBaseUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointBaseUri,
            _ => Constants.DevelopmentEndpointBaseUri,
        };

        Uri requestUri = new(baseUri, "/address/v1/addresses/resolve");

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, requestUri);

        httpRequest.Content = JsonContent.Create(request, FedExJsonSerializerContext.Default.FedExAddressValidationRequest);

        using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        if ( response.IsSuccessStatusCode )
        {
            return await response.Content.ReadFromJsonAsync(ApiJsonSerializerContext.Default.ApiResponse,
                                      cancellationToken)
                                 .ConfigureAwait(false);
        }

        ApiErrorResponse? errorResponse = await response.Content.ReadFromJsonAsync(ApiJsonSerializerContext.Default.ApiErrorResponse,
                                                             cancellationToken)
                                                        .ConfigureAwait(false);

        if ( errorResponse is not null )
        {
            return new ApiResponse
            {
                ErrorResponse = errorResponse,
            };
        }

        return null;
    }
}
