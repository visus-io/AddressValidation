namespace Visus.AddressValidation.Integration.Ups.Clients;

using System.Net.Http.Json;
using Configuration;
using Contracts;
using Microsoft.Extensions.Options;
using Serialization.Json;

internal sealed class UpsAddressValidationClient
{
    private readonly HttpClient _httpClient;

    private readonly IOptions<UpsServiceOptions> _options;

    public UpsAddressValidationClient(HttpClient httpClient, IOptions<UpsServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<ApiResponse?> ValidateAddressAsync(ApiRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateAddressInternalAsync(request, cancellationToken);
    }

    private async Task<ApiResponse?> ValidateAddressInternalAsync(ApiRequest request, CancellationToken cancellationToken)
    {
        Uri requestUri = new(_options.Value.EndpointBaseUri, "/api/addressvalidation/v2/3");

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, requestUri);

        httpRequest.Content = JsonContent.Create(request, ApiRequestJsonSerializerContext.Default.ApiRequest);

        using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        if ( response.IsSuccessStatusCode )
        {
            return await response.Content.ReadFromJsonAsync(ApiResponseJsonSerializerContext.Default.ApiResponse,
                                      cancellationToken)
                                 .ConfigureAwait(false);
        }

        ApiErrorResponse? errorResponse = await response.Content.ReadFromJsonAsync(ApiResponseJsonSerializerContext.Default.ApiErrorResponse,
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
