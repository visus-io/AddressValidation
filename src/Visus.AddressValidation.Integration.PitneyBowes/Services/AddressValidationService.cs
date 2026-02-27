namespace Visus.AddressValidation.Integration.PitneyBowes.Services;

using AddressValidation.Services;
using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationService : AbstractAddressValidationService<PitneyBowesAddressValidationRequest, ApiResponse>
{
    private readonly PitneyBowesAddressValidationClient _client;

    public AddressValidationService(PitneyBowesAddressValidationClient client,
                                    IValidator<PitneyBowesAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestValidator, responseValidator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override async ValueTask<ApiResponse?> SendAsync(PitneyBowesAddressValidationRequest request, CancellationToken cancellationToken)
    {
        return await _client.ValidateAddressAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
