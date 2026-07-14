namespace Visus.AddressValidation.Integration.FedEx.Adapters;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using Clients;
using Contracts;
using Models;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchApiRequestAdapter : IBatchApiRequestAdapter<FedExAddressValidationRequest, ApiResponse>
{
    private readonly FedExAddressValidationClient _client;

    private readonly IBatchApiRequestMapper<FedExAddressValidationRequest, ApiRequest> _requestMapper;

    public BatchApiRequestAdapter(FedExAddressValidationClient client,
                                  IBatchApiRequestMapper<FedExAddressValidationRequest, ApiRequest> requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
    }

    public Task<ApiResponse?> ExecuteAsync(IReadOnlyList<FedExAddressValidationRequest> requests, CancellationToken cancellationToken)
    {
        ApiRequest apiRequest = _requestMapper.Map(requests);
        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
