namespace Visus.AddressValidation.Integration.Ups.Http;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using AddressValidation.Abstractions;
using Microsoft.Extensions.Configuration;
using Serialization.Json;

internal sealed class UpsAddressValidationClient(
    IConfiguration configuration,
    HttpClient httpClient)
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<ApiResponse?> ValidateAddressAsync(UpsAddressValidationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateAddressInternalAsync(request, cancellationToken);
    }

    private async ValueTask<ApiResponse?> ValidateAddressInternalAsync(UpsAddressValidationRequest request, CancellationToken cancellationToken)
    {
        if ( !Enum.TryParse(_configuration[Constants.ClientEnvironmentConfigurationKey], out ClientEnvironment clientEnvironment) )
        {
            clientEnvironment = ClientEnvironment.DEVELOPMENT;
        }

        Uri baseUri = clientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointBaseUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointBaseUri,
            _ => Constants.DevelopmentEndpointBaseUri
        };

        Uri requestUri = new(baseUri, "/api/addressvalidation/v2/3");

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
                ErrorResponse = errorResponse
            };
        }

        return null;
    }
}
