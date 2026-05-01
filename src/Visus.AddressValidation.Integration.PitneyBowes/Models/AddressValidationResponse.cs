namespace Visus.AddressValidation.Integration.PitneyBowes.Model;

using AddressValidation.Validation;
using Contracts;
using Models;

internal sealed class AddressValidationResponse : AbstractAddressValidationResponse<ApiResponse>
{
    public AddressValidationResponse(ApiResponse response, IValidationResult? validationResult = null)
        : base(response, validationResult)
    {
    }
}
