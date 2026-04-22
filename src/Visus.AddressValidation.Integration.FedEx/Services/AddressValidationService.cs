namespace Visus.AddressValidation.Integration.FedEx.Services;

using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Mappers;
using Model;

internal sealed class AddressValidationService : AbstractAddressValidationService<FedExAddressValidationRequest, ApiResponse>
{
    private readonly FedExAddressValidationClient _client;

    public AddressValidationService(FedExAddressValidationClient client,
                                    IValidator<FedExAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestValidator, responseValidator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override Task<ApiResponse?> SendAsync(FedExAddressValidationRequest request,
                                                    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApiRequest apiRequest = request.ToApiRequest();

        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
