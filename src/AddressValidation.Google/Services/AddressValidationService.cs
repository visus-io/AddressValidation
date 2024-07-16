namespace Visus.AddressValidation.Google.Services;

using FluentValidation;
using Http;

internal sealed class AddressValidationService(
	GoogleAddressValidationClient client,
	IValidator<GoogleAddressValidationRequest> requestValidator,
	IValidator<ApiAddressValidationResponse> responseValidator)
	: AbstractAddressValidationService<GoogleAddressValidationRequest, ApiAddressValidationResponse>(requestValidator, responseValidator)
{
	private readonly GoogleAddressValidationClient _client = client ?? throw new ArgumentNullException(nameof(client));

	protected override async ValueTask<ApiAddressValidationResponse?> SendAsync(GoogleAddressValidationRequest request, CancellationToken cancellationToken)
	{
		return await _client.ValidateAddressAsync(request, cancellationToken);
	}
}
