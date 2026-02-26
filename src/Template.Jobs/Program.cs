using Template.Jobs;

var builder = Host.CreateApplicationBuilder(args);
var environment = builder.Environment;
if (environment.IsEnvironment("Local"))
{
    builder.Configuration.AddUserSecrets<Program>();
}
var services = builder.Services;

services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
