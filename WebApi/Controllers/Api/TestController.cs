using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok($"Hello OpenTelemetry! ticks: {DateTime.Now.Ticks.ToString()[^3..]}\nTriggered from: {Request.Host}");
    }

    [HttpGet("gc")]
    public IActionResult TriggerGC()
    {
        GC.Collect();
        return Ok("Garbage collection triggered");
    }

    [HttpGet("cpu")]
    public IActionResult GenerateCpuLoad()
    {
        var sw = Stopwatch.StartNew();
        double sum = 0;
        var rnd = new Random();
        while (sw.Elapsed.TotalSeconds < 5)
        {
            sum += Math.Sqrt(rnd.NextDouble());
        }
        return Ok($"CPU load generated. Computation result: {sum}");
    }

    [HttpGet("alloc")]
    public IActionResult AllocateMemory()
    {
        var arrays = new byte[50][];
        for (int i = 0; i < 50; i++)
        {
            arrays[i] = new byte[1024 * 1024]; // 1 МБ каждый
        }
        return Ok("Allocated 50 MB of memory");
    }
    
    [HttpGet("exception")]
    public IActionResult ThrowException()
    {
        throw new Exception("This is an exception");
    }
}