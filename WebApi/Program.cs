using System.Diagnostics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("My App", serviceInstanceId: Environment.MachineName))
    .WithMetrics(meterBuilder => meterBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddPrometheusExporter()  // Экспорт метрик в формате Prometheus
    );

var app = builder.Build();

// Добавляем endpoint для скрейпинга метрик Prometheus (по умолчанию /metrics)
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/", () => "Hello OpenTelemetry! ticks:" + DateTime.Now.Ticks.ToString()[^3..]);

// Endpoint для принудительного запуска сборки мусора
app.MapGet("/gc", () =>
{
    GC.Collect();
    return "Garbage collection triggered";
});

// Endpoint для создания нагрузки на CPU (работает 5 секунд)
app.MapGet("/cpu", () =>
{
    var sw = Stopwatch.StartNew();
    double sum = 0;
    var rnd = new Random();
    while (sw.Elapsed.TotalSeconds < 5)
    {
        sum += Math.Sqrt(rnd.NextDouble());
    }
    return $"CPU load generated. Computation result: {sum}";
});

// Endpoint для выделения памяти (50 МБ)
app.MapGet("/alloc", () =>
{
    var arrays = new byte[50][];
    for (int i = 0; i < 50; i++)
    {
        arrays[i] = new byte[1024 * 1024]; // 1 МБ каждый
    }
    return "Allocated 50 MB of memory";
});

app.Run();
