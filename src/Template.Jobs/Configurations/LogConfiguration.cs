using Serilog;
using Serilog.Formatting.Compact;

namespace Template.Jobs.Configurations;

internal static class LogConfiguration
{
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        services.AddSerilog((serviceProvider, logConfiguration) =>
        {
            var enviroment = serviceProvider.GetRequiredService<IHostEnvironment>();

            logConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning);

            if (enviroment.IsEnvironment("Local"))
            {
                logConfiguration
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}");
            }
            else
            {
                logConfiguration
                .MinimumLevel.Override("Npgsql.Command", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console(new CompactJsonFormatter());
            }
        });
        return services;
    }
}