using Coravel;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using Serilog;
using Template.Jobs.Schedulers;

namespace Template.Jobs.Configurations;

internal static class SchedulerConfiguration
{
    public static IServiceCollection AddSchedulers(this IServiceCollection services)
    {
        services.AddTransient<Job1Worker>();
        services.AddTransient<Worker>();
        services.AddScheduler();
        return services;
    }

    public static void UseSchedulers(this IHost app)
    {
        var options = app.Services.GetRequiredService<IOptionsMonitor<SchedulerSettings>>();
        var refresher = app.Services.GetRequiredService<IConfigurationRefresher>();
        app.Services.UseScheduler(scheduler =>
        {
            scheduler
            .Schedule<Worker>()
            .EverySecond();

            options.OnChange(option =>
            {
                UnsubscribeJobs(scheduler, option);
                SubscribeJobs(scheduler, option);
            });

            SubscribeJobs(scheduler, options.CurrentValue);

            scheduler.ScheduleAsync(async () =>
            {
                await refresher.RefreshAsync();
            }).EveryTenSeconds();
        }).LogScheduledTaskProgress()
        .OnError(ex => Log.Error(ex, "Problem on scheduled service"));
    }

    private static void SubscribeJobs(IScheduler scheduler, SchedulerSettings option)
    {
        scheduler
            .Schedule<Job1Worker>()
            .Cron(option.Job1.CronExpression)
            .PreventOverlapping(nameof(option.Job1))
            .When(() => Task.FromResult(option.Job1.IsEnabled));
    }

    private static void UnsubscribeJobs(IScheduler scheduler, SchedulerSettings option)
    {
        foreach (var job in option.Jobs)
        {
            (scheduler as Scheduler)!.TryUnschedule(job.Key);
        }
    }
}