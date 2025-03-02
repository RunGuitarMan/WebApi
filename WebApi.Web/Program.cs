using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebApi.Application;
using WebApi.Persistence;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "MyApp";
const string jaegerEndpoint = "http://localhost:4317";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName, serviceNamespace: "localhost", serviceInstanceId: Environment.MachineName))
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
        .AddSource(serviceName)
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(jaegerEndpoint);
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
    );

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = serviceName;
});

builder.Services.AddPersistence();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();

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