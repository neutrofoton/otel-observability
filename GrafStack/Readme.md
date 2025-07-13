# Comparison of Prometheus, Loki, and Tempo

| Component   | Main Function                   | Data Type           | Example Use Case                                     | Default Storage                    | Full-Text Search | Alternative Storage / Integration       |
|-------------|----------------------------------|----------------------|------------------------------------------------------|------------------------------------|------------------|-----------------------------------------|
| **Prometheus** | Monitoring and time-series metrics | Metrics (numeric + time) | Monitor CPU, memory, request/sec, set alerts         | Internal TSDB (local disk)         | ❌ No             | Thanos, Cortex, VictoriaMetrics         |
| **Loki**       | Log aggregation (structured logs)  | Logs (text)          | View errors, debug logs, filter by labels            | Chunked logs + index (disk or S3)  | ❌ Limited        | S3, GCS, Azure, Cassandra, MinIO        |
| **Tempo**      | Distributed tracing                | Traces (request paths) | Trace end-to-end flow of requests across services     | Object storage (S3, GCS, Azure)    | ❌ No             | BoltDB index, GCS, S3, MinIO            |

## Summary

- **Prometheus** is used for collecting and querying numerical metrics about system performance.
- **Loki** is used for centralized logging, optimized for label-based queries (lighter than ELK).
- **Tempo** is used to trace distributed requests across microservices, visualized in Grafana.

## Notes

- None of them use **Elasticsearch** as storage.
- All three are designed to work **together** as a complete **observability stack**.
- Data from all of them can be visualized in **Grafana dashboards**.
- Especially recommended for **Kubernetes** and **microservices-based systems**.


In this example
| Komponen   | URL                                            | Username / Password |
| ---------- | ---------------------------------------------- | ------------------- |
| Grafana    | [http://localhost:3000](http://localhost:3000) | `admin` / `admin`   |
| Prometheus | [http://localhost:9090](http://localhost:9090) | -                   |
| Loki API   | [http://localhost:3100](http://localhost:3100) | -                   |
| Tempo API  | [http://localhost:3200](http://localhost:3200) | -                   |

## 🧪 Observability Simulation in Microservices (.NET)

| Pillar     | Configuration Needed in Code                                             | Result (Visible In)               | Required NuGet Packages                                                                 |
|------------|--------------------------------------------------------------------------|-----------------------------------|------------------------------------------------------------------------------------------|
| **Metrics**| `AddOpenTelemetry().WithMetrics(...)` with ASP.NET + Runtime instrument  | ✅ Prometheus & Grafana dashboard | - `OpenTelemetry.Extensions.Hosting`<br>- `OpenTelemetry.Instrumentation.AspNetCore`<br>- `OpenTelemetry.Instrumentation.Runtime`<br>- `OpenTelemetry.Exporter.OpenTelemetryProtocol` |
| **Traces** | `AddOpenTelemetry().WithTracing(...)` with ASP.NET & HttpClient          | ✅ Tempo & Grafana Trace tab      | - `OpenTelemetry.Extensions.Hosting`<br>- `OpenTelemetry.Instrumentation.AspNetCore`<br>- `OpenTelemetry.Instrumentation.Http`<br>- `OpenTelemetry`<br>- `OpenTelemetry.Exporter.OpenTelemetryProtocol` |
| **Logs**   | `builder.Logging.AddOpenTelemetry(...)` + `ILogger` usage in endpoints   | ✅ Loki & Grafana Explore         |  `OpenTelemetry.Exporter.OpenTelemetryProtocol` |

# Note
- Provisioning datasource on Grafana makes us does not need to register datasource manually