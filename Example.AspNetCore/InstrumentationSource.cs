using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Example.AspNetCore;

public sealed class InstrumentationSource : IDisposable
{
    internal const string ActivitySourceName = "Example.AspNetCore";
    internal const string MeterName = "Example.AspNetCore";
    private readonly Meter _meter;

    public InstrumentationSource()
    {
        var version = typeof(InstrumentationSource).Assembly.GetName().Version?.ToString();
        this.ActivitySource = new ActivitySource(ActivitySourceName, version);
        this._meter = new Meter(MeterName, version);
        this.FreezingDaysCounter = this._meter.CreateCounter<long>(
            "weather.days.freezing",
            description: "The number of days where the temperature is below freezing."
        );
    }

    public ActivitySource ActivitySource { get; }
    public Counter<long> FreezingDaysCounter { get; }

    public void Dispose()
    {
        this.ActivitySource.Dispose();
        this._meter.Dispose();
    }
}
