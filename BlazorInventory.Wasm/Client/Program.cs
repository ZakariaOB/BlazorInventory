using BlazorInventory.Data;
using BlazorInventory.Data.Repository;
using BlazorInventory.Wasm;
using BlazorInventory.Wasm.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


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
            builder.Services.AddDbContextFactory<ClientSideDbContext>(
                options => options.UseSqlite($"Filename={DataSynchronizer.SqliteDbFilename}"));
            builder.Services.AddScoped<DataSynchronizer>();

            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IItemRevisedRepository, ItemRevisedRepository>();
            builder.Services.AddScoped<IGroupRepository, GroupRepository>();
            builder.Services.AddScoped<ISubGroupRepository, SubGroupRepository>();

            await builder.Build().RunAsync();
        }
    }
}