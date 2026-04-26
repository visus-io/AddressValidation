namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using AddressValidation.Mappers;
using Http;
using Model;

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
        };
    }
}
