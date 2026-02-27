namespace Visus.AddressValidation.Integration.FedEx.Services;

using AddressValidation.Services;
using AddressValidation.Validation;
using Http;

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

    protected override async ValueTask<ApiResponse?> SendAsync(FedExAddressValidationRequest request, CancellationToken cancellationToken)
    {
        return await _client.ValidateAddressAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
