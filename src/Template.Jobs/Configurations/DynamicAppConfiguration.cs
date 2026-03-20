using System.Net.Mime;
using System.Text.Json;
using Azure.Data.AppConfiguration;

namespace Template.Jobs.Configurations;

internal static class DynamicAppConfiguration
{
    public static async Task AddDynamicConfiguration(this IServiceCollection services, IConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("AppConfiguration");
        var schedularSettings = configuration.GetSection(SchedulerSettings.Key).Get<SchedulerSettings>();

        if (!string.IsNullOrEmpty(connectionString) && schedularSettings is { Jobs.Count: > 0 })
        {
            await SetupAzureAppConfiguration(connectionString, schedularSettings);
            AddAzureAppConfiguration(services, configuration);
        }
        services.Configure<SchedulerSettings>(configuration.GetSection(SchedulerSettings.Key));
    }

    private static void AddAzureAppConfiguration(IServiceCollection services, IConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("AppConfiguration");

        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(connectionString)
            .Select($"{SchedulerSettings.Key}:*", SchedulerSettings.Label)
            .ConfigureRefresh(refreshOptions =>
            {
                refreshOptions.RegisterAll().SetRefreshInterval(TimeSpan.FromSeconds(10));
            });
            services.AddSingleton(options.GetRefresher());
        });
    }

    private static async Task SetupAzureAppConfiguration(string connectionString, SchedulerSettings scheduler)
    {
        var client = new ConfigurationClient(connectionString);
        var configurationSetting = GetConfigurationSetting(client);

        var schedulerSettings = TransformConfigurationSetting(scheduler);
        await DeleteIfNotUsed(schedulerSettings, client, configurationSetting);
        await CreateIfIsNew(schedulerSettings, client, configurationSetting);
    }

    private static async Task CreateIfIsNew(IEnumerable<ConfigurationSetting> schedulerSettings, ConfigurationClient client, IEnumerable<ConfigurationSetting> configurationSetting)
    {
        var configsToAdd = schedulerSettings.ExceptBy(configurationSetting.Select(c => c.Key), c => c.Key);

        foreach (var schedulerSetting in configsToAdd)
        {
            await client.SetConfigurationSettingAsync(schedulerSetting, onlyIfUnchanged: true);
        }
    }

    private static async Task DeleteIfNotUsed(IEnumerable<ConfigurationSetting> schedulerSettings, ConfigurationClient client, IEnumerable<ConfigurationSetting> configurationSetting)
    {
        var configsToRemove = configurationSetting.ExceptBy(schedulerSettings.Select(c => c.Key), c => c.Key);

        foreach (var schedulerSetting in configsToRemove.Select(x => x.Key))
        {
            await client.DeleteConfigurationSettingAsync(schedulerSetting, SchedulerSettings.Label);
        }
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

    private static IEnumerable<ConfigurationSetting> TransformConfigurationSetting(SchedulerSettings scheduler)
    {
        return scheduler.Jobs
            .Select(job => new ConfigurationSetting($"{SchedulerSettings.Key}:{job.Key}", JsonSerializer.Serialize(job.Value, JsonSerializerOptions))
            {
                ContentType = MediaTypeNames.Application.Json,
                Label = SchedulerSettings.Label
            });
    }

    private static IEnumerable<ConfigurationSetting> GetConfigurationSetting(ConfigurationClient client)
    {
        var selector = new SettingSelector { LabelFilter = SchedulerSettings.Label };

        var configurationSettings = client
            .GetConfigurationSettingsAsync(selector)
            .ToBlockingEnumerable();
        return configurationSettings;
    }
}
