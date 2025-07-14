using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

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

var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello from {Service} at {Time}", "Order Service", DateTime.UtcNow);
    return Results.Ok("Hello from Order Service");
});

await app.RunAsync();
