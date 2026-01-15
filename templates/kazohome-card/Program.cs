using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using KazoHomeCard;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// TODO: Add KazoHome services when SDK is published
// builder.Services.AddKazoHomeBlazor();

await builder.Build().RunAsync();
