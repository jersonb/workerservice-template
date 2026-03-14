using Coravel.Invocable;
using Dapper;
using Npgsql;

namespace Template.Jobs.Schedulers;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : IInvocable
{
    public async Task Invoke()
    {
        var connection = new NpgsqlConnection(configuration.GetConnectionString("Postgres"));

        var ok = await connection.QueryAsync<bool>("select 1");

        if (logger.IsEnabled(LogLevel.Information))
        {
            var test = configuration.GetConnectionString("DefaultConnection");
            logger.LogInformation("Worker running at: {Time}, {Test}", DateTimeOffset.Now, test);
        }
    }
}