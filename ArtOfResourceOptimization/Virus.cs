namespace ArtOfResourceOptimization;

public class WeatherForecast
{
    public DateOnly Date { get; }
    public int TemperatureC { get; }
    public string? Summary { get; }

    public WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class Virus
{
    private static bool _isRunning = false;
    private static List<WeatherForecast> dangerList2 = new List<WeatherForecast>();

    public Virus()
    {
        // if (_isRunning)
        // {
        //     if (Random.Shared.Next(0, 2) is 0 or 1)
        //     {
        //         ThreadUsage();
        //     }
        //     return;
        // }

        // ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        // if (minWorkerThreads != 1000)
        // {
        //     ThreadPool.SetMaxThreads(1, 1);
        //     ThreadPool.SetMinThreads(1, 1);
        // }

        // _isRunning = true;
        // MemoryLeak();
        // CpuUsage();
        // ThreadUsage();
    }

    private void ThreadUsage()
    {
        _ = Task.Run(() =>
        {
            for (int i = 0; i < 10_000; i++)
            {
                _ = Task.Run(() =>
                {
                    Thread.Sleep(1000);
                });
            }
        });
    }

    private void CpuUsage()
    {
        _ = Task.Run(() =>
        {
            while (true)
            {
                var s = 1000 * 123_123;
                s = 1000 * 323_323;
                s = 0;
            }
        });
        _ = Task.Run(() =>
        {
            while (true)
            {
                var s = 1000 * 123_123;
                s = 1000 * 323_323;
                s = 0;
            }
        });
        _ = Task.Run(() =>
        {
            while (true)
            {
                var s = 1000 * 123_123;
                s = 1000 * 323_323;
                s = 0;
            }
        });
    }

    private void MemoryLeak()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        for (int i = 0; i < 2_000_000; i++)
        {
            dangerList2.Add(new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ));
        }
    }
}