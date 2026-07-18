namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using AddressValidation.Mappers;
using AddressValidation.Models;
using AddressValidation.Validation;
using Contracts;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchAddressValidationResponseMapper : IBatchApiResponseMapper<ApiResponse>
{
    public IAddressValidationResponse Map(ApiResponse response, int index, IValidationResult? validationResult = null)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        if ( response.Result is null || index >= response.Result.ResolvedAddresses.Length )
        {
            return new EmptyAddressValidationResponse(validationResult);
        }

        return ResolvedAddressResponseMapper.Map(response, response.Result.ResolvedAddresses[index], validationResult);
    }
}
