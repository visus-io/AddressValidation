namespace Visus.AddressValidation.Diagnostics;

/// <summary>
///     Provides the name shared by the <see cref="System.Diagnostics.ActivitySource" /> and
///     <see cref="System.Diagnostics.Metrics.Meter" /> used to emit traces and metrics for address validation
///     operations.
/// </summary>
/// <remarks>
///     Pass <see cref="SourceName" /> to <c>AddSource</c> and <c>AddMeter</c> when configuring OpenTelemetry to
///     collect traces and metrics emitted by this library.
/// </remarks>
public static class AddressValidationTelemetry
{
    /// <summary>The name shared by this library's <see cref="System.Diagnostics.ActivitySource" /> and <see cref="System.Diagnostics.Metrics.Meter" />.</summary>
    public const string SourceName = "Visus.AddressValidation";
}
