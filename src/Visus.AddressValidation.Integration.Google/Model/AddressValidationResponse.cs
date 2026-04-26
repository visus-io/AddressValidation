namespace Visus.AddressValidation.Integration.Google.Model;

using AddressValidation.Model;
using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationResponse : AbstractAddressValidationResponse<ApiResponse>
{
    public AddressValidationResponse(ApiResponse response, IValidationResult? validationResult = null)
        : base(response, validationResult)
    {
    }
}
