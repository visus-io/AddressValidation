namespace Visus.AddressValidation.Integration.FedEx.Services;

using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Model;

internal sealed class AddressValidationService
    : AbstractAddressValidationService<FedExAddressValidationRequest, ApiRequest, ApiResponse>
{
    private readonly FedExAddressValidationClient _client;

    public AddressValidationService(FedExAddressValidationClient client,
                                    IValidator<FedExAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IApiRequestMapper<FedExAddressValidationRequest, ApiRequest> requestMapper)
        : base(requestValidator, responseValidator, responseMapper, requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override Task<ApiResponse?> SendAsync(ApiRequest request, CancellationToken cancellationToken)
    {
        return _client.ValidateAddressAsync(request, cancellationToken);
    }
}
