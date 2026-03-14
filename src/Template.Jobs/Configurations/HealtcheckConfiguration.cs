using Dapper;
using Npgsql;
using System.Net;
using System.Text.Json.Serialization;
using TinyHealthCheck;
using TinyHealthCheck.HealthChecks;
using TinyHealthCheck.Models;

namespace Template.Jobs.Configurations;

internal static class HealtcheckConfiguration
{
    public static IServiceCollection AddHealhcheck(this IServiceCollection services)
    {
        services.AddCustomTinyHealthCheck<CustomHealthCheck>(config =>
        {
            config.Port = 8080;
            config.UrlPath = "/healthz";
            config.Hostname = "*";
            return config;
        });
        return services;
    }
}

internal class CustomHealthCheck(IConfiguration configuration) : IHealthCheck
{
    public async Task<IHealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

        IEnumerable<Task<HealthCheckState>> tasks = [
            CheckDatabase(cancellationToken),
        ];

        var checks = await Task.WhenAll(tasks);
        var result = new HealthCheckResult(checks.ToDictionary(x => x.Name, x => x.Check));
        return new JsonHealthCheckResult(result, result.StatusCode);
    }

    private async Task<HealthCheckState> CheckDatabase(CancellationToken cancellationToken)
    {
        try
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            ArgumentNullException.ThrowIfNull(connectionString);

            var ok = await new NpgsqlConnection(connectionString).QueryFirstAsync<bool>("select 1", cancellationToken);

            return new HealthCheckState("postgres", ok);
        }
        catch
        {
            return new HealthCheckState("postgres", false);
        }
    }

    private sealed record HealthCheckState(string Name, bool Check);
}

public class HealthCheckResult(IDictionary<string, bool> components)
{
    [JsonPropertyName("status")]
    public string Status { get; } = CheckStatus(components.Values.All(x => x));

    [JsonPropertyName("components")]
    public Dictionary<string, string> Components { get; } = components.ToDictionary(x => x.Key, x => CheckStatus(x.Value));

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; } = components.Values.All(x => x) ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable;

    private static string CheckStatus(bool check)
        => check ? "healthy" : "unhealth";
}"