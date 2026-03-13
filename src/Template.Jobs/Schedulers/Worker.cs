using Coravel.Invocable;

namespace Template.Jobs.Schedulers;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : IInvocable
{
    public Task Invoke()
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            var test = configuration.GetConnectionString("DefaultConnection");
            logger.LogInformation("Worker running at: {Time}, {Test}", DateTimeOffset.Now, test);
        }

        return Task.CompletedTask;
    }
}