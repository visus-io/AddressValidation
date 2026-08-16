namespace Visus.AddressValidation.Diagnostics;

using Models;

internal static class AddressValidationServiceDiagnostics
{
    internal const string s_resultError = "error";

    internal const string s_resultInvalidRequest = "invalid_request";

    internal const string s_resultInvalidResponse = "invalid_response";

    internal const string s_resultNoResponse = "no_response";

    internal const string s_resultSuccess = "success";

    internal const string s_tagCountry = "address_validation.country";

    internal const string s_tagRequestType = "address_validation.request_type";

    internal const string s_tagResult = "address_validation.result";

    internal const string s_unknownCountry = "unknown";

    internal static string CountryTag(AbstractAddressValidationRequest request)
    {
        return request.Country?.ToString() ?? s_unknownCountry;
    }

    internal static void RecordResponseCounts(string requestTypeName, string result, string country, IAddressValidationResponse? response)
    {
        if ( response is null )
        {
            return;
        }

        AddressValidationDiagnostics.ResponseWarningCount.Record(
            response.Warnings.Count,
            new KeyValuePair<string, object?>(s_tagRequestType, requestTypeName),
            new KeyValuePair<string, object?>(s_tagResult, result),
            new KeyValuePair<string, object?>(s_tagCountry, country));

        AddressValidationDiagnostics.ResponseSuggestionCount.Record(
            response.Suggestions.Count,
            new KeyValuePair<string, object?>(s_tagRequestType, requestTypeName),
            new KeyValuePair<string, object?>(s_tagResult, result),
            new KeyValuePair<string, object?>(s_tagCountry, country));
    }
}
