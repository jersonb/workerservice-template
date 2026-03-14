using Template.Jobs.Data;

namespace Template.Jobs.Configurations;

internal static class DatabaseConfiguration
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");

        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddNpgsqlDataSource(connectionString, builder =>
        {
            builder.EnableParameterLogging();
            builder.ConfigureTracing(options => options.EnableFirstResponseEvent());
        });

        services.AddScoped<DatabaseProvider>();
        return services;
    }
}