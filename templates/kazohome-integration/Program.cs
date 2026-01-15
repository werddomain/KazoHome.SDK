using KazoHomeIntegration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<IntegrationService>();

// TODO: Add KazoHome Bridge services when SDK is published
// builder.Services.AddKazoHomeBridge(options =>
// {
//     options.SocketPath = "/tmp/kazohome_integration.sock";
// });

var host = builder.Build();
host.Run();
