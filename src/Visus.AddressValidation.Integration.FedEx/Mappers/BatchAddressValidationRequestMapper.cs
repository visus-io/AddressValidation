namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using AddressValidation.Mappers;
using Contracts;
using Models;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchAddressValidationRequestMapper : IBatchApiRequestMapper<FedExAddressValidationRequest, ApiRequest>
{
    public ApiRequest Map(IReadOnlyList<FedExAddressValidationRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);

        // FedEx's resolve endpoint accepts a single CustomerTransactionId per call, not one per address, so only
        // the first request's value can be transmitted; use ClientReferenceId for per-item correlation instead.
        return new ApiRequest
        {
            AddressesToValidate = [.. requests.Select(FedExAddressToValidateMapper.Map),],
            CustomerTransactionId = requests.Count > 0 ? requests[0].CustomerTransactionId : null,
        };
    }
}
