namespace Visus.AddressValidation.Integration.Ups.Services;

using AddressValidation.Services;
using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationService : AbstractAddressValidationService<UpsAddressValidationRequest, ApiResponse>
{
    private readonly UpsAddressValidationClient _client;

    public AddressValidationService(UpsAddressValidationClient client,
                                    IValidator<UpsAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator) 
        : base(requestValidator, responseValidator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override async ValueTask<ApiResponse?> SendAsync(UpsAddressValidationRequest request, CancellationToken cancellationToken)
    {
        return await _client.ValidateAddressAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
