namespace Visus.AddressValidation.Integration.PitneyBowes.Mappers;

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using AddressValidation.Mappers;
using AddressValidation.Validation;
using Contracts;
using Model;
using Models;

internal sealed class AddressValidationResponseMapper : IApiResponseMapper<ApiResponse>
{
    public IAddressValidationResponse Map(ApiResponse response, IValidationResult? validationResult = null)
    {
        if ( response.Result is null )
        {
            return new EmptyAddressValidationResponse(validationResult);
        }

        return new AddressValidationResponse(response, validationResult)
        {
            AddressLines = response.Result.AddressLines
                                   .ToFrozenSet(StringComparer.OrdinalIgnoreCase),
            CityOrTown = response.Result.CityTown,
            Country = response.Result.CountryCode,
            PostalCode = response.Result.PostalCode,
            StateOrProvince = response.Result.StateProvince,
            IsResidential = response.Result.Residential,
            Suggestions = ListSuggestions(response),
            CustomResponseData = response.Result.GetCustomResponseData(),
        };
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
