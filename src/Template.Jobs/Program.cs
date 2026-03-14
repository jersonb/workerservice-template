using Template.Jobs.Configurations;

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

services.AddSchedulers();

var host = builder.Build();

host.UseSchedulers();

await host.RunAsync();