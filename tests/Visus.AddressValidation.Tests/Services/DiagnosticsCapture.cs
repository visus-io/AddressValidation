namespace Visus.AddressValidation.Tests.Services;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Diagnostics;

/// <summary>
///     Captures <see cref="Activity" /> and <see cref="Measurement" /> instances emitted through
///     <see cref="AddressValidationDiagnostics" /> for the lifetime of the instance.
/// </summary>
internal sealed class DiagnosticsCapture : IDisposable
{
    private readonly ConcurrentQueue<Activity> _activities = new();

    private readonly ActivityListener _activityListener;

    private readonly ConcurrentQueue<Measurement> _measurements = new();

    private readonly MeterListener _meterListener;

    public DiagnosticsCapture()
    {
        _activityListener = new ActivityListener
        {
            ShouldListenTo = source => string.Equals(source.Name, AddressValidationTelemetry.SourceName, StringComparison.Ordinal),
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => _activities.Enqueue(activity),
        };
        ActivitySource.AddActivityListener(_activityListener);

        _meterListener = new MeterListener
        {
            InstrumentPublished = (instrument, listener) =>
            {
                if ( string.Equals(instrument.Meter.Name, AddressValidationTelemetry.SourceName, StringComparison.Ordinal) )
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            },
        };
        _meterListener.SetMeasurementEventCallback<double>((instrument, value, tags, _) => _measurements.Enqueue(new Measurement(instrument.Name, value, tags.ToArray())));
        _meterListener.SetMeasurementEventCallback<long>((instrument, value, tags, _) => _measurements.Enqueue(new Measurement(instrument.Name, value, tags.ToArray())));
        _meterListener.Start();
    }

    public IReadOnlyList<Activity> Activities => [.. _activities,];

    public IReadOnlyList<Measurement> Measurements => [.. _measurements,];

    public void Dispose()
    {
        _activityListener.Dispose();
        _meterListener.Dispose();
    }

    internal readonly record struct Measurement(string InstrumentName, double Value, KeyValuePair<string, object?>[] Tags);
}
