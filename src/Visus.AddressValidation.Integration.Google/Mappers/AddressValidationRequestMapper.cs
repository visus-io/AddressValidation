namespace Visus.AddressValidation.Integration.Google.Mappers;

using Http;
using Model;

internal static class AddressValidationRequestMapper
{
    internal static ApiRequest ToApiRequest(this GoogleAddressValidationRequest request)
    {
        return new ApiRequest
        {
            Address = new ApiRequest.GoogleAddress
            {
                AddressLines = [..request.AddressLines,],
                AdministrativeArea = request.StateOrProvince,
                Locality = request.CityOrTown,
                PostalCode = request.PostalCode,
                RegionCode = request.Country!.Value,
            },
            EnableUspsCass = request.EnableUspsCass,
            PreviousResponseId = request.PreviousResponseId,
        };
    }
}
