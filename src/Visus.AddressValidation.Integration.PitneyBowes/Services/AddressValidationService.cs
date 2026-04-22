namespace Visus.AddressValidation.Integration.PitneyBowes.Services;

using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Mappers;
using Model;

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

    protected override Task<ApiResponse?> SendAsync(PitneyBowesAddressValidationRequest request,
                                                    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApiRequest apiRequest = request.ToApiRequest();

        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
