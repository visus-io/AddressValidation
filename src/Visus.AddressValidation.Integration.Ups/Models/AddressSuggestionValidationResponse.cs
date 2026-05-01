namespace Visus.AddressValidation.Integration.Ups.Models;

using System.Collections.Frozen;
using Abstractions;
using AddressValidation.Models;
using Contracts;
using Mappers;

internal sealed class AddressSuggestionValidationResponse : AbstractAddressValidationResponse
{
    public AddressSuggestionValidationResponse(ApiResponse.Candidate candidate)
    {
        AddressLines = candidate.AddressKeyFormat.AddressLine
                                .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        CityOrTown = candidate.AddressKeyFormat.PoliticalDivision2;
        Country = candidate.AddressKeyFormat.CountryCode;
        PostalCode = AddressValidationResponseMapper.FormatPostalCode(candidate.AddressKeyFormat);
        StateOrProvince = candidate.AddressKeyFormat.PoliticalDivision1;
        IsResidential = candidate.AddressClassification.Code == AddressClassificationCode.RESIDENTIAL;
    }
}
