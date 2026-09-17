# otel-sandbox

A sandbox for exploring OpenTelemetry and observability concepts — how apps behave in production, how signals flow through layers, and what good instrumentation looks like. The app itself is intentionally simple; it's just a vehicle.

## Prerequisites

- [.NET 10](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

## Modes

The sandbox runs in two modes. The OTel pipeline is the same in both — only the destination changes.

### Aspire (for local monitoring)

Run the AppHost. Aspire starts the app and its own dashboard, no Docker needed.

```bash
cd OtelSandbox.AppHost
dotnet run
```

| UI | URL |
| --- | --- |
| Aspire Dashboard | <http://localhost:15888> |
| Scalar (API docs) | <http://localhost:5000/scalar> |

### LGTM - Loki, Grafana, Tempo, Mimir & Prometheus (for production-like environments)

Start the LGTM stack services with docker, then run the app. Telemetry is exported to Grafana.

```bash
docker compose up -d
cd Example.AspNetCore
dotnet run
```

| UI | URL |
| --- | --- |
| Grafana (traces, logs, metrics) | <http://localhost:3000> |
| Prometheus | <http://localhost:9090> |
| Scalar (API docs) | <http://localhost:5000/scalar> |

## Configuration

The OTLP endpoint is controlled by a single key in `appsettings.Development.json`:

```json
{
  "OTEL_EXPORTER_OTLP_ENDPOINT": "http://localhost:4317"
}
```

Under Aspire this is injected automatically — you don't set it manually.

## Endpoints

``` text
POST /todos        create a todo
GET  /todos        list all todos
GET  /todos/{id}   get a todo by id
```
