namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using AddressValidation.Mappers;
using AddressValidation.Models;
using AddressValidation.Validation;
using Contracts;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationResponseMapper : IApiResponseMapper<ApiResponse>
{
    public IAddressValidationResponse Map(ApiResponse response, IValidationResult? validationResult = null)
    {
        ArgumentNullException.ThrowIfNull(response);

        if ( response.Result is null || response.Result.ResolvedAddresses.Length == 0 )
        {
            return new EmptyAddressValidationResponse(validationResult);
        }

        ApiResponse.ResolvedAddress primary = response.Result.ResolvedAddresses[0];

        return ResolvedAddressResponseMapper.Map(response, primary, validationResult);
    }
}
