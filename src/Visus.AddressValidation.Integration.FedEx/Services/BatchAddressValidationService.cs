namespace Visus.AddressValidation.Integration.FedEx.Services;

using AddressValidation.Adapters;
using AddressValidation.Mappers;
using AddressValidation.Services;
using AddressValidation.Validation;
using Contracts;
using Models;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchAddressValidationService
    : AbstractBatchAddressValidationService<FedExAddressValidationRequest, ApiResponse>
{
    public BatchAddressValidationService(IBatchApiRequestAdapter<FedExAddressValidationRequest, ApiResponse> batchRequestAdapter,
                                         IBatchApiResponseMapper<ApiResponse> batchResponseMapper,
                                         IValidator<FedExAddressValidationRequest> requestValidator,
                                         IBatchValidator<ApiResponse> batchResponseValidator)
        : base(batchRequestAdapter, batchResponseMapper, requestValidator, batchResponseValidator)
    {
    }

    protected override int MaxBatchSize => Constants.MaxBatchSize;
}
