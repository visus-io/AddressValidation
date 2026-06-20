namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using AddressValidation.Mappers;
using Contracts;
using Models;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationRequestMapper : IApiRequestMapper<FedExAddressValidationRequest, ApiRequest>
{
    public ApiRequest Map(FedExAddressValidationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ApiRequest
        {
            AddressesToValidate =
            [
                new ApiRequest.FedExAddressToValidate
                {
                    Address = new ApiRequest.FedExAddress
                    {
                        StreetLines = [..request.AddressLines,],
                        City = request.CityOrTown,
                        StateOrProvince = request.StateOrProvince,
                        PostalCode = request.PostalCode,
                        CountryCode = request.Country!.Value,
                    },
                    ClientReferenceId = request.ClientReferenceId,
                },
            ],
            CustomerTransactionId = request.CustomerTransactionId,
        };
    }
}
