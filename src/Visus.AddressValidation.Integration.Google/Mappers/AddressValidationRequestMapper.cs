namespace Visus.AddressValidation.Integration.Google.Mappers;

using AddressValidation.Mappers;
using Http;
using Model;

internal sealed class AddressValidationRequestMapper : IApiRequestMapper<GoogleAddressValidationRequest, ApiRequest>
{
    public ApiRequest Map(GoogleAddressValidationRequest request)
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
