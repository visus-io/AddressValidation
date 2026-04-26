namespace Visus.AddressValidation.Integration.Ups.Mappers;

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Mappers;
using AddressValidation.Model;
using AddressValidation.Validation;
using Http;
using Model;

internal sealed class AddressValidationResponseMapper : IApiResponseMapper<ApiResponse>
{
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

    public IAddressValidationResponse Map(ApiResponse response, IValidationResult? validationResult = null)
    {
        if ( response.Result is null || response.Result.Candidates.Length == 0 )
        {
            return new EmptyAddressValidationResponse(validationResult);
        }

        ApiResponse.Candidate primary = response.Result.Candidates[0];

        IReadOnlyList<IAddressValidationResponse> suggestions = response.Result.Candidates.Length > 1
                                                                    ? ListSuggestions(response.Result.Candidates.Skip(1))
                                                                    : [];

        return new AddressValidationResponse(response, validationResult)
        {
            AddressLines = primary.AddressKeyFormat.AddressLine
                                  .ToFrozenSet(StringComparer.OrdinalIgnoreCase),
            CityOrTown = primary.AddressKeyFormat.PoliticalDivision2,
            Country = primary.AddressKeyFormat.CountryCode,
            PostalCode = FormatPostalCode(primary.AddressKeyFormat),
            StateOrProvince = primary.AddressKeyFormat.PoliticalDivision1,
            IsResidential = primary.AddressClassification.Code == AddressClassificationCode.RESIDENTIAL,
            Suggestions = suggestions,
        };
    }

    private static ReadOnlyCollection<IAddressValidationResponse> ListSuggestions(IEnumerable<ApiResponse.Candidate> candidates)
    {
        HashSet<AddressSuggestionValidationResponse> results =
            [.. candidates.Select(s => new AddressSuggestionValidationResponse(s)),];

        return new ReadOnlyCollection<IAddressValidationResponse>([.. results,]);
    }
}
