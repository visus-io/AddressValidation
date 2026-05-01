namespace Visus.AddressValidation.Integration.Ups.Services;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Contracts;
using Models;

internal sealed class AddressValidationService :
    AbstractAddressValidationService<UpsAddressValidationRequest, ApiResponse>
{
    public AddressValidationService(IApiRequestAdapter<UpsAddressValidationRequest, ApiResponse> requestAdapter,
                                    IApiResponseMapper<ApiResponse> responseMapper,
                                    IValidator<UpsAddressValidationRequest> requestValidator,
                                    IValidator<ApiResponse> responseValidator)
        : base(requestAdapter, responseMapper, requestValidator, responseValidator)
    {
    }
}
