namespace Visus.AddressValidation.Integration.Google.Services;

using System.Diagnostics.CodeAnalysis;
using AddressValidation.Adapters;
using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Contracts;
using Models;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
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
