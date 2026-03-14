using Coravel.Invocable;
using Template.Jobs.Data;

namespace Template.Jobs.Schedulers;

internal class Worker(ILogger<Worker> logger, DatabaseProvider database) : IInvocable, ICancellableInvocable
{
    public CancellationToken CancellationToken { get; set; } = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

    public async Task Invoke()
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            var resut = await database.GetFirtsOrDefault<bool>("select 1", CancellationToken);
            logger.LogInformation("execute {Result}", resut);
        }
    }
}