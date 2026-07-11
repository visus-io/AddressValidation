---
title: Instrumentation
---

# Instrumentation

Every integration emits traces and metrics via [`System.Diagnostics`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing) (`ActivitySource` / `Meter`), which any OpenTelemetry-compatible collection pipeline can consume directly. Both the `ActivitySource` and the `Meter` share the name in [`AddressValidationTelemetry.SourceName`](xref:Visus.AddressValidation.Diagnostics.AddressValidationTelemetry) (`"Visus.AddressValidation"`).

## Setup

Register the [OpenTelemetry SDK](https://opentelemetry.io/docs/languages/net/) and point it at the shared source name via `AddSource` / `AddMeter`:

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithTracing(tracing => tracing.AddSource(AddressValidationTelemetry.SourceName))
    .WithMetrics(metrics => metrics.AddMeter(AddressValidationTelemetry.SourceName));
```

This requires the `OpenTelemetry.Extensions.Hosting` package:

```shell
dotnet package add OpenTelemetry.Extensions.Hosting
```

> [!NOTE]
> The snippet above only registers the source and meter — it does not send data anywhere. Add an exporter to `WithTracing` / `WithMetrics` as shown in [Exporting to a Backend](#exporting-to-a-backend) below.

## What Is Measured

### Activities

| Name | Emitted By | Tags | Description |
|---|---|---|---|
| `address_validation.validate` | [`AbstractAddressValidationService<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Services.AbstractAddressValidationService`2) | `address_validation.request_type`, `address_validation.country`, `address_validation.result` | Wraps a single `ValidateAsync` call, from request validation through response mapping. |
| `address_validation.token_fetch` | [`AbstractAuthenticationService<TClient>`](xref:Visus.AddressValidation.Services.AbstractAuthenticationService`1) | `address_validation.client_type`, `address_validation.result` | Wraps a single OAuth 2.0 token fetch on a cache miss. Not started on a cache hit. |

Both activities are marked with `ActivityStatusCode.Error` and record the exception when the underlying operation throws.

### Metrics

| Name | Kind | Unit | Tags | Description |
|---|---|---|---|---|
| `visus.address_validation.validate.duration` | Histogram\<double\> | s | request_type, result, country | Duration of a `ValidateAsync` call. |
| `visus.address_validation.validate.response_warning_count` | Histogram\<long\> | — | request_type, result, country | Number of warnings on the produced response. Not recorded when the API returns no response. |
| `visus.address_validation.validate.response_suggestion_count` | Histogram\<long\> | — | request_type, result, country | Number of suggestions on the produced response. Not recorded when the API returns no response. |
| `visus.address_validation.token_fetch.duration` | Histogram\<double\> | s | client_type, result | Duration of a token fetch (cache miss only). |
| `visus.address_validation.token_fetch.cache_result` | Counter\<long\> | — | client_type, cache_result | Incremented on every token cache lookup, whether it hits or misses. |

### Tag Values

| Tag | Values |
|---|---|
| `address_validation.result` (validate) | `success`, `invalid_request`, `no_response`, `invalid_response`, `error` |
| `address_validation.result` (token_fetch) | `success`, `empty_token`, `error` |
| `address_validation.cache_result` | `hit`, `miss` |
| `address_validation.request_type` | The `TRequest` type name (e.g., `FedExAddressValidationRequest`) |
| `address_validation.client_type` | The `TClient` type name of the provider's authentication client |
| `address_validation.country` | The [`CountryCode`](xref:Visus.AddressValidation.Abstractions.CountryCode) of the request, or `unknown` when absent |

## Exporting to a Backend

The tabs below each extend the [Setup](#setup) snippet with an exporter. Pick the one that matches your observability stack.

# [Console](#tab/tab-ave-otel-console)

For local development and debugging — writes traces and metrics to standard output.

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithTracing(tracing => tracing
        .AddSource(AddressValidationTelemetry.SourceName)
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddMeter(AddressValidationTelemetry.SourceName)
        .AddConsoleExporter());
```

```shell
dotnet package add OpenTelemetry.Exporter.Console
```

# [OTLP Collector](#tab/tab-ave-otel-otlp)

Sends data to any collector that accepts the [OpenTelemetry Protocol](https://opentelemetry.io/docs/specs/otlp/) — including [Jaeger](https://www.jaegertracing.io/) and [Grafana Tempo](https://grafana.com/oss/tempo/) — via a local [OpenTelemetry Collector](https://opentelemetry.io/docs/collector/).

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithTracing(tracing => tracing
        .AddSource(AddressValidationTelemetry.SourceName)
        .AddOtlpExporter(otlp => otlp.Endpoint = new Uri("http://localhost:4317")))
    .WithMetrics(metrics => metrics
        .AddMeter(AddressValidationTelemetry.SourceName)
        .AddOtlpExporter(otlp => otlp.Endpoint = new Uri("http://localhost:4317")));
```

```shell
dotnet package add OpenTelemetry.Exporter.OpenTelemetryProtocol
```

# [Azure Monitor](#tab/tab-ave-otel-azure)

Sends data directly to [Azure Monitor Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable) using its connection string — no collector required.

```csharp
string connectionString = builder.Configuration["ApplicationInsights:ConnectionString"]!;

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithTracing(tracing => tracing
        .AddSource(AddressValidationTelemetry.SourceName)
        .AddAzureMonitorTraceExporter(azure => azure.ConnectionString = connectionString))
    .WithMetrics(metrics => metrics
        .AddMeter(AddressValidationTelemetry.SourceName)
        .AddAzureMonitorMetricExporter(azure => azure.ConnectionString = connectionString));
```

```shell
dotnet package add Azure.Monitor.OpenTelemetry.Exporter
```

# [Datadog](#tab/tab-ave-otel-datadog)

The [Datadog Agent](https://docs.datadoghq.com/opentelemetry/setup/agentless/) accepts OTLP directly, so this reuses the standard OTLP exporter pointed at the agent's OTLP intake port.

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithTracing(tracing => tracing
        .AddSource(AddressValidationTelemetry.SourceName)
        .AddOtlpExporter(otlp => otlp.Endpoint = new Uri("http://localhost:4318")))
    .WithMetrics(metrics => metrics
        .AddMeter(AddressValidationTelemetry.SourceName)
        .AddOtlpExporter(otlp => otlp.Endpoint = new Uri("http://localhost:4318")));
```

```shell
dotnet package add OpenTelemetry.Exporter.OpenTelemetryProtocol
```

> [!NOTE]
> The Datadog Agent must have OTLP ingestion enabled (`otlp_config.receiver.protocols.grpc` in `datadog.yaml`, or the `DD_OTLP_CONFIG_RECEIVER_PROTOCOLS_GRPC_ENDPOINT` environment variable). See the [Datadog OTLP ingestion docs](https://docs.datadoghq.com/opentelemetry/setup/otlp_ingest_in_the_agent/) for details.

# [New Relic](#tab/tab-ave-otel-newrelic)

[New Relic](https://docs.newrelic.com/docs/opentelemetry/best-practices/opentelemetry-otlp/) accepts OTLP directly — no collector required. Authenticate with a [license key](https://docs.newrelic.com/docs/apis/intro-apis/new-relic-api-keys/) sent as the `api-key` header.

```csharp
string licenseKey = builder.Configuration["NewRelic:LicenseKey"]!;

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithTracing(tracing => tracing
        .AddSource(AddressValidationTelemetry.SourceName)
        .AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri("https://otlp.nr-data.net:4317");
            otlp.Headers = $"api-key={licenseKey}";
        }))
    .WithMetrics(metrics => metrics
        .AddMeter(AddressValidationTelemetry.SourceName)
        .AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri("https://otlp.nr-data.net:4317");
            otlp.Headers = $"api-key={licenseKey}";
        }));
```

```shell
dotnet package add OpenTelemetry.Exporter.OpenTelemetryProtocol
```

> [!NOTE]
> Use `https://otlp.eu01.nr-data.net:4317` instead if your account is hosted in the EU data center. See the [New Relic OTLP endpoint docs](https://docs.newrelic.com/docs/opentelemetry/best-practices/opentelemetry-otlp/#review-settings) for details.

# [AWS (X-Ray / CloudWatch)](#tab/tab-ave-otel-aws)

Sends data to a local [AWS Distro for OpenTelemetry (ADOT) Collector](https://aws-otel.github.io/docs/getting-started/collector), which forwards traces to [X-Ray](https://aws.amazon.com/xray/) and metrics to [CloudWatch](https://aws.amazon.com/cloudwatch/).

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithTracing(tracing => tracing
        .AddSource(AddressValidationTelemetry.SourceName)
        .AddOtlpExporter(otlp => otlp.Endpoint = new Uri("http://localhost:4317")))
    .WithMetrics(metrics => metrics
        .AddMeter(AddressValidationTelemetry.SourceName)
        .AddOtlpExporter(otlp => otlp.Endpoint = new Uri("http://localhost:4317")));
```

```shell
dotnet package add OpenTelemetry.Exporter.OpenTelemetryProtocol
```

> [!NOTE]
> Configuring the ADOT Collector itself (X-Ray and CloudWatch exporters, IAM permissions) is outside the scope of this library. See the [ADOT Collector documentation](https://aws-otel.github.io/docs/getting-started/collector) for setup instructions.

# [Prometheus + Grafana](#tab/tab-ave-otel-prometheus)

Exposes metrics on a scrape endpoint for [Prometheus](https://prometheus.io/) to pull, visualized in [Grafana](https://grafana.com/). Traces still need an OTLP target such as Tempo — see the OTLP Collector tab above.

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithMetrics(metrics => metrics
        .AddMeter(AddressValidationTelemetry.SourceName)
        .AddPrometheusExporter());

// After building the app:
app.MapPrometheusScrapingEndpoint();
```

```shell
dotnet package add OpenTelemetry.Exporter.Prometheus.AspNetCore
```

---
