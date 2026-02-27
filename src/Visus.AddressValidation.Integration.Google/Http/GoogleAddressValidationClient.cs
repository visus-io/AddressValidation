namespace Visus.AddressValidation.Integration.Google.Http;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Serialization.Json;

internal sealed class GoogleAddressValidationClient
{
    private readonly HttpClient _httpClient;

    public GoogleAddressValidationClient(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = Constants.ProductionEndpointBaseUri;
        _httpClient.DefaultRequestHeaders.Add("X-Goog-User-Project", configuration[Constants.ProjectIdConfigurationKey]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<ApiResponse?> ValidateAddressAsync(GoogleAddressValidationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateAddressInternalAsync(request, cancellationToken);
    }

    private async ValueTask<ApiResponse?> ValidateAddressInternalAsync(GoogleAddressValidationRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/v1:validateAddress",
                                                                   request,
                                                                   GoogleJsonSerializerContext.Default.GoogleAddressValidationRequest,
                                                                   cancellationToken)
                                                              .ConfigureAwait(false);
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
