namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Mappers;
using Model;

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

    protected override Task<ApiResponse?> SendAsync(GoogleAddressValidationRequest request,
                                                    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApiRequest apiRequest = request.ToApiRequest();

        return _client.ValidateAddressAsync(apiRequest, cancellationToken);
    }
}
