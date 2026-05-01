namespace Visus.AddressValidation.Integration.Google.Adapters;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using Clients;
using Contracts;
using Models;

internal sealed class ApiRequestAdapter : IApiRequestAdapter<GoogleAddressValidationRequest, ApiResponse>
{
    private readonly GoogleAddressValidationClient _client;

    private readonly IApiRequestMapper<GoogleAddressValidationRequest, ApiRequest> _requestMapper;

    public ApiRequestAdapter(GoogleAddressValidationClient client,
                             IApiRequestMapper<GoogleAddressValidationRequest, ApiRequest> requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
    }

    public Task<ApiResponse?> ExecuteAsync(GoogleAddressValidationRequest request, CancellationToken cancellationToken)
    {
        ApiRequest apiRequest = _requestMapper.Map(request);
        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
