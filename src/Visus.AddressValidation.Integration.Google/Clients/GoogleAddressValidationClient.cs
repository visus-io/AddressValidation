namespace Visus.AddressValidation.Integration.Google.Clients;

using System.Net.Http.Json;
using Configuration;
using Contracts;
using Microsoft.Extensions.Options;
using Serialization.Json;

internal sealed class GoogleAddressValidationClient
{
    private static readonly Uri ProductionEndpointBaseUri = new("https://addressvalidation.googleapis.com");

    private readonly HttpClient _httpClient;

    public GoogleAddressValidationClient(HttpClient httpClient, IOptions<GoogleServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = ProductionEndpointBaseUri;
        _httpClient.DefaultRequestHeaders.Add("X-Goog-User-Project", options.Value.ProjectId);
    }

    public Task<ApiResponse?> ValidateAddressAsync(ApiRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateAddressInternalAsync(request, cancellationToken);
    }

    private async Task<ApiResponse?> ValidateAddressInternalAsync(ApiRequest request,
                                                                  CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/v1:validateAddress",
                                                                   request,
                                                                   ApiRequestJsonSerializerContext.Default.ApiRequest,
                                                                   cancellationToken)
                                                              .ConfigureAwait(false);
        if ( response.IsSuccessStatusCode )
        {
            return await response.Content.ReadFromJsonAsync(ApiResponseJsonSerializerContext.Default.ApiResponse,
                                      cancellationToken)
                                 .ConfigureAwait(false);
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
