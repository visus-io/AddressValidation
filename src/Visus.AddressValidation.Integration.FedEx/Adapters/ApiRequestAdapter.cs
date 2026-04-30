namespace Visus.AddressValidation.Integration.FedEx.Adapters;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using Http;
using Model;

internal sealed class ApiRequestAdapter : IApiRequestAdapter<FedExAddressValidationRequest, ApiResponse>
{
    private readonly FedExAddressValidationClient _client;

    private readonly IApiRequestMapper<FedExAddressValidationRequest, ApiRequest> _requestMapper;

    public ApiRequestAdapter(FedExAddressValidationClient client,
                             IApiRequestMapper<FedExAddressValidationRequest, ApiRequest> requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
    }

    public Task<ApiResponse?> ExecuteAsync(FedExAddressValidationRequest request, CancellationToken cancellationToken)
    {
        ApiRequest apiRequest = _requestMapper.Map(request);
        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
