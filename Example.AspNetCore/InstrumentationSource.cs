using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Example.AspNetCore;

public sealed class InstrumentationSource : IDisposable
{
    internal const string ActivitySourceName = "Example.AspNetCore";
    internal const string MeterName = "Example.AspNetCore";
    private readonly Meter _meter;

    public InstrumentationSource()
    {
        var version = typeof(InstrumentationSource).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        _meter = new Meter(MeterName, version);

        CreateDuration = _meter.CreateHistogram<double>(
            "todos.create.duration",
            unit: "ms",
            description: "Time taken to create a todo."
        );

        TodosCreatedCounter = _meter.CreateCounter<long>(
            "todos.created",
            description: "The number of todos created."
        );
    }

    public ActivitySource ActivitySource { get; }

    public Counter<long> TodosCreatedCounter { get; }
    public Histogram<double> CreateDuration;

    public void Dispose()
    {
        this.ActivitySource.Dispose();
        this._meter.Dispose();
    }

    public static void AddMetrics(MeterProviderBuilder metrics)
    {
        metrics.AddMeter(MeterName);
    }

    public static void AddTracing(TracerProviderBuilder tracing)
    {
        tracing.AddSource(ActivitySourceName);
    }
}
