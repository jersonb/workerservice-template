using TinyHealthCheck;

namespace Template.Jobs.Configurations;

internal static class HealtcheckConfiguration
{
    public static IServiceCollection AddHealhcheck(this IServiceCollection services)
    {
        services.AddBasicTinyHealthCheck(config =>
        {
            config.Port = 8080;
            config.UrlPath = "/healthz";
            config.Hostname = "*";
            return config;
        });
        return services;
    }
}