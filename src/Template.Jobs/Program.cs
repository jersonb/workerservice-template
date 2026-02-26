using Template.Jobs;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
