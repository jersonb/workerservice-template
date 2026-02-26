namespace Template.Jobs;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                var test = configuration.GetConnectionString("DefaultConnection");
                logger.LogInformation("Worker running at: {Time}, {Test}", DateTimeOffset.Now, test);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}