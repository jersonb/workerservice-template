using Coravel.Invocable;

namespace Template.Jobs.Schedulers;

internal class Job1Worker(ILogger<Job1Worker> logger) : IInvocable, ICancellableInvocable
{
    public CancellationToken CancellationToken { get; set; } = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

    public async Task Invoke()
    {
        logger.LogWarning("Executing Job1Worker");
    }
}