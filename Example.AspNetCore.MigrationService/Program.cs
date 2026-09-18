using Example.AspNetCore.Data;
using Example.AspNetCore.MigrationService;
using OtelSandbox.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder
    .Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<AppDbContext>("todoDb");

var host = builder.Build();
host.Run();
