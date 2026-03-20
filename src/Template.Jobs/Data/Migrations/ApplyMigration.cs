using DbUp;
using System.Reflection;

namespace Template.Jobs.Data.Migrations;

internal class ApplyMigration(IConfiguration configuration, ILogger<ApplyMigration> logger, IHost lifetime) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            ArgumentNullException.ThrowIfNull(connectionString);

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var upgradeResult = upgrader.PerformUpgrade();
            if (!upgradeResult.Successful)
            {
                logger.LogError(upgradeResult.Error, "Erro on try aplly postgres migrations");
                await lifetime.StopAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Problem on execute postgres migration");
            await lifetime.StopAsync(cancellationToken);
        }

        logger.LogInformation("Starting apply postgres migration");
        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}