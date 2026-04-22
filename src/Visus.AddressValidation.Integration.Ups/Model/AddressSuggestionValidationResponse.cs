namespace Visus.AddressValidation.Integration.Ups.Model;

using System.Collections.Frozen;
using Abstractions;
using AddressValidation.Model;
using Http;

internal sealed class AddressSuggestionValidationResponse : AbstractAddressValidationResponse
{
    public AddressSuggestionValidationResponse(ApiResponse.Candidate candidate)
    {
        AddressLines = candidate.AddressKeyFormat.AddressLine
                                .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        CityOrTown = candidate.AddressKeyFormat.PoliticalDivision2;
        Country = candidate.AddressKeyFormat.CountryCode;
        PostalCode = AddressValidationResponse.FormatPostalCode(candidate.AddressKeyFormat);
        StateOrProvince = candidate.AddressKeyFormat.PoliticalDivision1;
        IsResidential = candidate.AddressClassification.Code == AddressClassificationCode.RESIDENTIAL;
    }
}
