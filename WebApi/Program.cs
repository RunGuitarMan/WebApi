using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "MyApp", serviceNamespace: "localhost", serviceInstanceId: Environment.MachineName))
    .WithMetrics(meterBuilder => meterBuilder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter()
    )
    .WithTracing(tracerBuilder => tracerBuilder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
        .AddSource("MyApp")
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("https://localhost:4317"); // Отправляем в OTEL-Collector
            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
    );

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();