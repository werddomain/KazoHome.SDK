using KazoHomeAddon;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<AddonWorker>();

// TODO: Add KazoHome services when SDK is published
// builder.Services.AddKazoHomeContext();

var host = builder.Build();
host.Run();
