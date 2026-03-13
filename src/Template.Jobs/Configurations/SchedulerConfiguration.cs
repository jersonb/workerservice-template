using Coravel;
using Template.Jobs.Schedulers;

namespace Template.Jobs.Configurations;

internal static class SchedulerConfiguration
{
    public static IServiceCollection AddSchedulers(this IServiceCollection services)
    {
        services.AddTransient<Worker>();
        services.AddScheduler();
        return services;
    }

    public static void UseSchedulers(this IHost app)
    {
        app.Services.UseScheduler(scheduler =>
        {
            scheduler
            .Schedule<Worker>()
            .EverySecond();
        }).LogScheduledTaskProgress()
        .OnError(ex => throw ex);
    }
}