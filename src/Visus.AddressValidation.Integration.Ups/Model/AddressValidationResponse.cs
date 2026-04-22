namespace Visus.AddressValidation.Integration.Ups.Model;

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Model;
using AddressValidation.Validation;
using Http;

internal sealed class AddressValidationResponse : AbstractAddressValidationResponse<ApiResponse>
{
    public AddressValidationResponse(ApiResponse response, IValidationResult? validationResult = null)
        : base(response, validationResult)
    {
        if ( response.Result is null )
        {
            return;
        }

        ApiResponse.Candidate primaryAddress = response.Result.Candidates[0];

        AddressLines = primaryAddress.AddressKeyFormat.AddressLine
                                     .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        CityOrTown = primaryAddress.AddressKeyFormat.PoliticalDivision2;
        Country = primaryAddress.AddressKeyFormat.CountryCode;
        PostalCode = FormatPostalCode(primaryAddress.AddressKeyFormat);
        StateOrProvince = primaryAddress.AddressKeyFormat.PoliticalDivision1;
        IsResidential = primaryAddress.AddressClassification.Code == AddressClassificationCode.RESIDENTIAL;

        if ( response.Result.Candidates.Length > 1 )
        {
            Suggestions = ListSuggestions(response.Result.Candidates.Skip(1));
        }
    }

    public static string? FormatPostalCode(ApiResponse.AddressKeyFormat addressKeyFormat)
    {
        if ( addressKeyFormat.CountryCode != CountryCode.US )
        {
            return addressKeyFormat.PostcodePrimaryLow;
        }

        HashSet<string?> codes = new(StringComparer.OrdinalIgnoreCase)
        {
            addressKeyFormat.PostcodePrimaryLow,
            addressKeyFormat.PostcodeExtendedLow,
        };

        return string.Join("-", [.. codes,]);
    }

    private static ReadOnlyCollection<IAddressValidationResponse> ListSuggestions(IEnumerable<ApiResponse.Candidate> candidates)
    {
        HashSet<AddressSuggestionValidationResponse> results =
            [.. candidates.Select(s => new AddressSuggestionValidationResponse(s)),];

        return new ReadOnlyCollection<IAddressValidationResponse>([.. results,]);
    }
}
