namespace Visus.AddressValidation.Diagnostics;

using System.Diagnostics;
using System.Diagnostics.Metrics;

internal static class AddressValidationDiagnostics
{
    public static readonly ActivitySource ActivitySource = new(AddressValidationTelemetry.SourceName);

    private static readonly Meter s_meter = new(AddressValidationTelemetry.SourceName);

    /// <summary>Records the number of access token cache lookups, tagged by cache hit/miss result.</summary>
    public static readonly Counter<long> CacheResultCounter = s_meter.CreateCounter<long>(
        "visus.address_validation.token_fetch.cache_result",
        description: "The number of access token cache lookups, tagged by cache hit/miss result.");

    /// <summary>Records the number of suggestions present on a produced address validation response.</summary>
    public static readonly Histogram<long> ResponseSuggestionCount = s_meter.CreateHistogram<long>(
        "visus.address_validation.validate.response_suggestion_count",
        description: "The number of suggestions returned on an address validation response.");

    /// <summary>Records the number of warnings present on a produced address validation response.</summary>
    public static readonly Histogram<long> ResponseWarningCount = s_meter.CreateHistogram<long>(
        "visus.address_validation.validate.response_warning_count",
        description: "The number of warnings returned on an address validation response.");

    /// <summary>Records the duration, in seconds, of token fetch operations.</summary>
    public static readonly Histogram<double> TokenFetchDuration = s_meter.CreateHistogram<double>(
        "visus.address_validation.token_fetch.duration",
        "s",
        "The duration of token fetch operations in seconds.");

    /// <summary>Records the duration, in seconds, of address validation operations.</summary>
    public static readonly Histogram<double> ValidationDuration = s_meter.CreateHistogram<double>(
        "visus.address_validation.validate.duration",
        "s",
        "The duration of address validation operations in seconds.");
}
