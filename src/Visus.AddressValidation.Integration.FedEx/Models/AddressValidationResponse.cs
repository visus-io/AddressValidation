namespace Visus.AddressValidation.Integration.FedEx.Models;

using AddressValidation.Models;
using AddressValidation.Validation;
using Contracts;

internal sealed class AddressValidationResponse : AbstractAddressValidationResponse<ApiResponse>
{
    public AddressValidationResponse(ApiResponse response, IValidationResult? validationResult = null)
        : base(response, validationResult)
    {
    }
}
