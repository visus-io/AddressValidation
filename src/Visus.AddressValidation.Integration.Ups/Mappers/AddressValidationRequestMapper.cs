namespace Visus.AddressValidation.Integration.Ups.Mappers;

using Http;
using Model;

internal static class AddressValidationRequestMapper
{
    internal static ApiRequest ToApiRequest(this UpsAddressValidationRequest request)
    {
        string[] postalCodeParts =
            request.PostalCode!.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new ApiRequest
        {
            XavRequest = new ApiRequest.UpsXavRequest
            {
                AddressKeyFormat = new ApiRequest.UpsAddressKeyFormat
                {
                    AddressLine = [..request.AddressLines,],
                    PoliticalDivision2 = request.CityOrTown,
                    PoliticalDivision1 = request.StateOrProvince,
                    PostcodePrimaryLow = postalCodeParts[0],
                    PostcodeExtendedLow = postalCodeParts.Length == 2 ? postalCodeParts[1] : null,
                    CountryCode = request.Country!.Value,
                },
            },
        };
    }
}
