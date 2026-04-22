namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using System.Net.Http.Json;
using Configuration;
using Microsoft.Extensions.Options;
using Serialization.Json;

internal sealed class PitneyBowesAddressValidationClient
{
    private readonly HttpClient _httpClient;

    private readonly IOptions<PitneyBowesServiceOptions> _options;

    public PitneyBowesAddressValidationClient(HttpClient httpClient, IOptions<PitneyBowesServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<ApiResponse?> ValidateAddressAsync(ApiRequest request,
                                                   CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateAddressInternalAsync(request, cancellationToken);
    }

    private async Task<ApiResponse?> ValidateAddressInternalAsync(ApiRequest request,
                                                                  CancellationToken cancellationToken)
    {
        Uri requestUri = request.IncludeSuggestions
                             ? new Uri(_options.Value.EndpointBaseUri,
                                 "/shippingservices/v1/addresses/verify-suggest?returnSuggestions=true")
                             : new Uri(_options.Value.EndpointBaseUri,
                                 "/shippingservices/v1/addresses/verify?minimalAddressValidation=false");

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, requestUri);

        httpRequest.Content = JsonContent.Create(request, ApiRequestJsonSerializerContext.Default.ApiRequest);
        httpRequest.Headers.Add("X-PB-UnifiedErrorStructure", "true");

        using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken)
                                                              .ConfigureAwait(false);

        if ( response.IsSuccessStatusCode )
        {
            if ( request.IncludeSuggestions )
            {
                return await response.Content.ReadFromJsonAsync(ApiResponseJsonSerializerContext.Default.ApiResponse,
                                          cancellationToken)
                                     .ConfigureAwait(false);
            }

            ApiResponse.AddressResult? result =
                await response.Content.ReadFromJsonAsync(ApiResponseJsonSerializerContext.Default.AddressResult,
                                   cancellationToken)
                              .ConfigureAwait(false);
            return new ApiResponse
            {
                Result = result,
            };
        }

        ApiErrorResponse? errorResponse =
            await response.Content.ReadFromJsonAsync(ApiResponseJsonSerializerContext.Default.ApiErrorResponse,
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
