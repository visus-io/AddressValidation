namespace Visus.AddressValidation.Integration.Ups.Services;

using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Model;

internal sealed class AddressValidationService :
    AbstractAddressValidationService<UpsAddressValidationRequest, ApiRequest, ApiResponse>
{
    private readonly UpsAddressValidationClient _client;

    public AddressValidationService(UpsAddressValidationClient client,
                                    IValidator<UpsAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IApiRequestMapper<UpsAddressValidationRequest, ApiRequest> requestMapper)
        : base(requestValidator, responseValidator, responseMapper, requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override Task<ApiResponse?> SendAsync(ApiRequest request, CancellationToken cancellationToken)
    {
        return _client.ValidateAddressAsync(request, cancellationToken);
    }
}
