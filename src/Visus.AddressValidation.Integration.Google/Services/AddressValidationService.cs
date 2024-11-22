namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Services;
using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationService(
	GoogleAddressValidationClient client,
	IValidator<GoogleAddressValidationRequest> requestValidator,
	IValidator<ApiResponse> responseValidator)
	: AbstractAddressValidationService<GoogleAddressValidationRequest, ApiResponse>(requestValidator, responseValidator)
{
	private readonly GoogleAddressValidationClient _client = client ?? throw new ArgumentNullException(nameof(client));

	protected override async ValueTask<ApiResponse?> SendAsync(GoogleAddressValidationRequest request, CancellationToken cancellationToken)
	{
		return await _client.ValidateAddressAsync(request, cancellationToken).ConfigureAwait(false);
	}
}
