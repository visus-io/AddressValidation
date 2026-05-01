namespace Visus.AddressValidation.Integration.Google.Services;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Contracts;
using Models;

internal sealed class AddressValidationService :
    AbstractAddressValidationService<GoogleAddressValidationRequest, ApiResponse>
{
    public AddressValidationService(IApiRequestAdapter<GoogleAddressValidationRequest, ApiResponse> requestAdapter,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IValidator<GoogleAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestAdapter, responseMapper, requestValidator, responseValidator)
    {
    }
}
