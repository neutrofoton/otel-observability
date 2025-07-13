using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Hapus provider default
builder.Logging.ClearProviders();

// Tambahkan OTEL Logging
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService("order-service")); // Ganti nama service sesuai microservice

    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    logging.ParseStateValues = true;

    logging.AddOtlpExporter(opt =>
    {
        opt.Endpoint = new Uri("http://otel-collector:4317");
    });
});


builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("order-service"))

    //Configurasi Metric untuk Prometheus
    /*
    - http.server.duration
    - process.runtime.dotnet.gc.count
    */
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddRuntimeInstrumentation();
        metrics.AddMeter("Microsoft.AspNetCore.Hosting");
        metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
        metrics.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri("http://otel-collector:4317");
        });
    })

    //Configurasi Trace untuk Tempo
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation(); // jika service saling panggil
        tracing.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri("http://otel-collector:4317");
        });
    })
    ;

var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello from {Service} at {Time}", "Order Service", DateTime.UtcNow);
    return Results.Ok("Hello from Order Service");
});

await app.RunAsync();
