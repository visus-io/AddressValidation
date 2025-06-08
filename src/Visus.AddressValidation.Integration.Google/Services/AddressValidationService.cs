namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Services;
using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationService : AbstractAddressValidationService<GoogleAddressValidationRequest, ApiResponse>
{
    private readonly GoogleAddressValidationClient _client;

    public AddressValidationService(GoogleAddressValidationClient client,
                                    IValidator<GoogleAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestValidator, responseValidator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override async ValueTask<ApiResponse?> SendAsync(GoogleAddressValidationRequest request, CancellationToken cancellationToken)
    {
        return await _client.ValidateAddressAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
