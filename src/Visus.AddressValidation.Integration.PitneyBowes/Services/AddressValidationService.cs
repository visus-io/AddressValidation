namespace Visus.AddressValidation.Integration.PitneyBowes.Services;

using System.Diagnostics.CodeAnalysis;
using AddressValidation.Adapters;
using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Contracts;
using Model;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
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
