namespace Visus.AddressValidation.Integration.FedEx.Mappers;

using Http;
using Model;

internal static class AddressValidationRequestMapper
{
    internal static ApiRequest ToApiRequest(this FedExAddressValidationRequest request)
    {
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
