namespace Visus.AddressValidation.Integration.Ups.Adapters;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using Http;
using Model;

internal sealed class ApiRequestAdapter : IApiRequestAdapter<UpsAddressValidationRequest, ApiResponse>
{
    private readonly UpsAddressValidationClient _client;

    private readonly IApiRequestMapper<UpsAddressValidationRequest, ApiRequest> _requestMapper;

    public ApiRequestAdapter(UpsAddressValidationClient client,
                             IApiRequestMapper<UpsAddressValidationRequest, ApiRequest> requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
    }

    public Task<ApiResponse?> ExecuteAsync(UpsAddressValidationRequest request, CancellationToken cancellationToken)
    {
        ApiRequest apiRequest = _requestMapper.Map(request);
        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
