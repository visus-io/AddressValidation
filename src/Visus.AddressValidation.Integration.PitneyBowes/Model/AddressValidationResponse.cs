namespace Visus.AddressValidation.Integration.PitneyBowes.Model;

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using AddressValidation.Abstractions;
using AddressValidation.Model;
using AddressValidation.Validation;
using Http;

internal sealed class AddressSuggestionValidationResponse : AbstractAddressValidationResponse
{
    public AddressSuggestionValidationResponse(ApiResponse.AddressResult addressResult, CountryCode countryCode, string? postalCode)
    {
        AddressLines = addressResult.AddressLines
                                    .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        CityOrTown = addressResult.CityTown;
        Country = countryCode;
        PostalCode = postalCode;
        StateOrProvince = addressResult.StateProvince;
        IsResidential = addressResult.Residential;
    }
}

internal sealed class AddressValidationResponse : AbstractAddressValidationResponse<ApiResponse>
{
    public AddressValidationResponse(ApiResponse response, IValidationResult? validationResult = null)
        : base(response, validationResult)
    {
        if ( response.Result is null )
        {
            return;
        }

        AddressLines = response.Result.AddressLines
                               .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        CityOrTown = response.Result.CityTown;
        Country = response.Result.CountryCode;
        PostalCode = response.Result.PostalCode;
        StateOrProvince = response.Result.StateProvince;
        IsResidential = response.Result.Residential;
        Suggestions = ListSuggestions(response);

        CustomResponseData = response.Result.GetCustomResponseData();
    }

    private static ReadOnlyCollection<IAddressValidationResponse> ListSuggestions(ApiResponse response)
    {
        if ( response.Result is null || response.Suggestions?.Addresses is null )
        {
            return [];
        }

        HashSet<AddressSuggestionValidationResponse> results =
        [
            .. response.Suggestions.Addresses
                       .Select(s => new AddressSuggestionValidationResponse(
                            s,
                            response.Result.CountryCode,
                            response.Result.PostalCode)
                        ),
        ];

        return new ReadOnlyCollection<IAddressValidationResponse>([.. results,]);
    }
}
