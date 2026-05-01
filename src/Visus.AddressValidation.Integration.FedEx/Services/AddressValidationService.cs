namespace Visus.AddressValidation.Integration.FedEx.Services;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Contracts;
using Models;

internal sealed class AddressValidationService
    : AbstractAddressValidationService<FedExAddressValidationRequest, ApiResponse>
{
    public AddressValidationService(IApiRequestAdapter<FedExAddressValidationRequest, ApiResponse> requestAdapter,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IValidator<FedExAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestAdapter, responseMapper, requestValidator, responseValidator)
    {
    }
}
