using System.Diagnostics.Metrics;
using Example.AspNetCore;
using Example.AspNetCore.Data;
using Example.AspNetCore.Endpoints;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var appBuilder = WebApplication.CreateBuilder(args);

var otlpEndpoint = new Uri(
    appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")
);

appBuilder.Logging.ClearProviders();

appBuilder
    .Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService(
            serviceName: appBuilder.Configuration.GetValue(
                "ServiceName",
                defaultValue: "otel-sandbox"
            ),
            serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "Unknown",
            serviceInstanceId: Environment.MachineName
        )
    )
    .WithTracing(builder =>
    {
        builder
            .AddSource(InstrumentationSource.ActivitySourceName)
            .SetSampler(new AlwaysOnSampler())
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(o => o.Endpoint = otlpEndpoint);

        appBuilder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(
            appBuilder.Configuration.GetSection("AspNetCoreInstrumentation")
        );
    })
    .WithMetrics(builder =>
    {
        builder
            .AddMeter(InstrumentationSource.MeterName)
            .SetExemplarFilter(ExemplarFilterType.TraceBased)
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(o => o.Endpoint = otlpEndpoint);

        builder.AddView(instrument =>
            instrument.GetType().GetGenericTypeDefinition() == typeof(Histogram<>)
                ? new Base2ExponentialBucketHistogramConfiguration()
                : null
        );
    })
    .WithLogging(builder =>
    {
        builder.AddOtlpExporter(o => o.Endpoint = otlpEndpoint);
    });

appBuilder.Services.AddSingleton<InstrumentationSource>();
appBuilder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
appBuilder.Services.AddOpenApi();
appBuilder.Services.AddAuthorization();

var app = appBuilder.Build();

app.MapTodoRoutes();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();
app.Run();
