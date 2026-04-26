namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Model;

internal sealed class AddressValidationService :
    AbstractAddressValidationService<GoogleAddressValidationRequest, ApiRequest, ApiResponse>
{
    private readonly GoogleAddressValidationClient _client;

    public AddressValidationService(GoogleAddressValidationClient client,
                                    IValidator<GoogleAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IApiRequestMapper<GoogleAddressValidationRequest, ApiRequest> requestMapper)
        : base(requestValidator, responseValidator, responseMapper, requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override Task<ApiResponse?> SendAsync(ApiRequest request, CancellationToken cancellationToken)
    {
        return _client.ValidateAddressAsync(request, cancellationToken);
    }
}
