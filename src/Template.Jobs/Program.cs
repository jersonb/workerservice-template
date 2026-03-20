using Template.Jobs.Configurations;
using Template.Jobs.Data.Migrations;

var builder = Host.CreateApplicationBuilder(args);

var environment = builder.Environment;
var services = builder.Services;
var configuration = builder.Configuration;

if (environment.IsEnvironment("Local"))
{
    configuration.AddUserSecrets(environment.ApplicationName);
}
else
{
    services.AddHealhcheck();
}

services.AddCustomLogging();

services.AddHostedService<ApplyMigration>();

services.ConfigureDatabase(configuration);

services.AddSchedulers();

await services.AddDynamicConfiguration(configuration);

var host = builder.Build();

host.UseSchedulers();

await host.RunAsync();