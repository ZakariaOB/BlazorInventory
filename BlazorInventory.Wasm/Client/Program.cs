using BlazorInventory.Data;
using BlazorInventory.Wasm;
using BlazorInventory.Wasm.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


namespace BlazorInventory
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            var backendOrigin = builder.Configuration["BackendOrigin"]!;
            builder.RootComponents.Add<App>("blazor-app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // gRPC-Web client
            builder.Services.AddManufacturingDataClient((services, options) =>
            {
                options.BaseUri = backendOrigin;
                options.MessageHandler = new HttpClientHandler();
            });

            // Sets up EF Core with Sqlite using QuickGrid
            builder.Services.AddManufacturingDataDbContext();
            await builder.Build().RunAsync();
        }
    }
}