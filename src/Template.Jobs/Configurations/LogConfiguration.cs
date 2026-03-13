using Serilog;

namespace Template.Jobs.Configurations;

internal static class LogConfiguration
{
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        services.AddSerilog((_, logConfiguration) =>
        {
            logConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}");
        });
        return services;
    }
}