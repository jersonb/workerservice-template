namespace Template.Jobs.Configurations;

internal class SchedulerSettings
{
    public static string Key => "Scheduler";
    public static string Label => "jobs";

    public JobConfiguration Job1 { get; set; } = new();

    public Dictionary<string, JobConfiguration> Jobs => new()
    {
        { nameof(Job1), Job1}
    };
}
internal class JobConfiguration
{
    public string CronExpression { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}