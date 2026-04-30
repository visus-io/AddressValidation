namespace Visus.AddressValidation.Integration.PitneyBowes.Services;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Http;
using Model;

internal sealed class AddressValidationService :
    AbstractAddressValidationService<PitneyBowesAddressValidationRequest, ApiResponse>
{
    public AddressValidationService(IApiRequestAdapter<PitneyBowesAddressValidationRequest, ApiResponse> requestAdapter,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IValidator<PitneyBowesAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestAdapter, responseMapper, requestValidator, responseValidator)
    {
    }
}
