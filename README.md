# otel-sandbox

A sandbox for exploring observability concepts with OpenTelemetry and the LGTM stack (Loki, Grafana, Tempo, Mimir).

## Prerequisites

- [.NET 10](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

## Getting started

1. Start the LGTM stack

```bash
docker compose up -d
```

2. Run the app

```bash
cd Example.AspNetCore
dotnet run
```

3. Generate some telemetry

Hit the weather endpoint a few times:

```bash
curl http://localhost:5000/WeatherForecast
```

## Where to look

| UI | URL |
| --- | --- |
| Grafana (traces, logs, metrics) | <http://localhost:3000> |
| Prometheus | <http://localhost:9090> |
| Swagger | <http://localhost:5000/swagger> |

## Switching exporters

The app defaults to OTLP. You can switch exporters in `appsettings.json`:

```json
"UseTracingExporter": "otlp",   // otlp | console
"UseMetricsExporter": "otlp",   // otlp | prometheus | console
"UseLogExporter": "otlp",       // otlp | console
"HistogramAggregation": "explicit" // explicit | exponential
```
