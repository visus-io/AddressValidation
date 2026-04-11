namespace Visus.AddressValidation.Integration.Ups.Http;

using System.Net.Http.Json;
using Configuration;
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

    public ValueTask<ApiResponse?> ValidateAddressAsync(UpsAddressValidationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateAddressInternalAsync(request, cancellationToken);
    }

    private async ValueTask<ApiResponse?> ValidateAddressInternalAsync(UpsAddressValidationRequest request, CancellationToken cancellationToken)
    {
        Uri requestUri = new(_options.Value.EndpointBaseUri, "/api/addressvalidation/v2/3");

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, requestUri);

        httpRequest.Content = JsonContent.Create(request, UpsJsonSerializerContext.Default.UpsAddressValidationRequest);

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
