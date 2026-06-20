namespace Visus.AddressValidation.Integration.Ups.Mappers;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AddressValidation.Mappers;
using Contracts;
using Models;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationRequestMapper : IApiRequestMapper<UpsAddressValidationRequest, ApiRequest>
{
    public ApiRequest Map(UpsAddressValidationRequest request)
    {
        string[] postalCodeParts =
            request.PostalCode!.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new ApiRequest
        {
            XavRequest = new ApiRequest.UpsXavRequest
            {
                Request = new ApiRequest.UpsRequest
                {
                    RequestOption = "3",
                },
                MaximumCandidateListSize = request.MaximumCandidateListSize?.ToString(CultureInfo.InvariantCulture),
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
