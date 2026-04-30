namespace Visus.AddressValidation.Integration.PitneyBowes.Adapters;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using Http;
using Model;

internal sealed class ApiRequestAdapter : IApiRequestAdapter<PitneyBowesAddressValidationRequest, ApiResponse>
{
    private readonly PitneyBowesAddressValidationClient _client;

    private readonly IApiRequestMapper<PitneyBowesAddressValidationRequest, ApiRequest> _requestMapper;

    public ApiRequestAdapter(PitneyBowesAddressValidationClient client,
                             IApiRequestMapper<PitneyBowesAddressValidationRequest, ApiRequest> requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
    }

    public Task<ApiResponse?> ExecuteAsync(PitneyBowesAddressValidationRequest request, CancellationToken cancellationToken)
    {
        ApiRequest apiRequest = _requestMapper.Map(request);
        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
