namespace Visus.AddressValidation.Integration.PitneyBowes.Services;

using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Model;

internal sealed class AddressValidationService :
    AbstractAddressValidationService<PitneyBowesAddressValidationRequest, ApiRequest, ApiResponse>
{
    private readonly PitneyBowesAddressValidationClient _client;

    public AddressValidationService(PitneyBowesAddressValidationClient client,
                                    IValidator<PitneyBowesAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IApiRequestMapper<PitneyBowesAddressValidationRequest, ApiRequest> requestMapper)
        : base(requestValidator, responseValidator, responseMapper, requestMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override Task<ApiResponse?> SendAsync(ApiRequest request, CancellationToken cancellationToken)
    {
        return _client.ValidateAddressAsync(request, cancellationToken);
    }
}
