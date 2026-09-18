using Example.AspNetCore;
using Example.AspNetCore.Data;
using Example.AspNetCore.Endpoints;
using Example.AspNetCore.Services;
using OtelSandbox.ServiceDefaults;
using Scalar.AspNetCore;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.AddServiceDefaults();

appBuilder
    .Services.AddOpenTelemetry()
    .WithMetrics(InstrumentationSource.AddMetrics)
    .WithTracing(InstrumentationSource.AddTracing);

appBuilder.AddNpgsqlDbContext<AppDbContext>("todoDb");

appBuilder.Services.AddSingleton<InstrumentationSource>();
appBuilder.Services.AddScoped<TodoService>();
appBuilder.Services.AddOpenApi();
appBuilder.Services.AddAuthorization();
var app = appBuilder.Build();

app.MapTodoEndpoints();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();
app.Run();
