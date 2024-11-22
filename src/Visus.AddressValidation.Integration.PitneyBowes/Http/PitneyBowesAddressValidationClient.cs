namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using AddressValidation.Abstractions;
using Microsoft.Extensions.Configuration;
using Serialization.Json;

internal sealed class PitneyBowesAddressValidationClient(
	IConfiguration configuration,
	HttpClient httpClient)
{
	private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

	private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueTask<ApiResponse?> ValidateAddressAsync(PitneyBowesAddressValidationRequest request, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);
		return ValidateAddressInternalAsync(request, cancellationToken);
	}

	private async ValueTask<ApiResponse?> ValidateAddressInternalAsync(PitneyBowesAddressValidationRequest request, CancellationToken cancellationToken)
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

		Uri requestUri = request.IncludeSuggestions
							 ? new Uri(baseUri, "/shippingservices/v1/addresses/verify-suggest?returnSuggestions=true")
							 : new Uri(baseUri, "/shippingservices/v1/addresses/verify?minimalAddressValidation=false");

		using HttpRequestMessage httpRequest = new(HttpMethod.Post, requestUri);

		httpRequest.Content = JsonContent.Create(request, PitneyBowesJsonSerializerContext.Default.PitneyBowesAddressValidationRequest);
		httpRequest.Headers.Add("X-PB-UnifiedErrorStructure", "true");

		using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
		if ( response.IsSuccessStatusCode )
		{
			if ( request.IncludeSuggestions )
			{
				return await response.Content.ReadFromJsonAsync(ApiJsonSerializerContext.Default.ApiResponse,
																cancellationToken)
									 .ConfigureAwait(false);
			}

			ApiResponse.AddressResult? result = await response.Content.ReadFromJsonAsync(ApiJsonSerializerContext.Default.AddressResult,
																						 cancellationToken)
															  .ConfigureAwait(false);
			return new ApiResponse
			{
				Result = result
			};
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
