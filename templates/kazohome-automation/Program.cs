using KazoHomeAutomation;
using KazoHomeAutomation.Automations;

var builder = Host.CreateApplicationBuilder(args);

// Register automations
builder.Services.AddHostedService<LightAutomation>();
builder.Services.AddHostedService<ClimateAutomation>();

// TODO: Add KazoHome services when SDK is published
// builder.Services.AddKazoHomeContext();

var host = builder.Build();
host.Run();
