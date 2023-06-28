using BlazorInventory.Server.Config;
using BlazorInventory.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorInventory.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString))
                .AddDatabaseDeveloperPageExceptionFilter();

            // Add services to the container.

            builder.Services.AddControllers();

            // Repositories
            builder.Services.ConfigureRepositories();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddGrpc();

            // Repositories


            var app = builder.Build();

            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

            // SeedData.EnsureSeeded(app.Services, withDataIncrement: false);
            SeedData.InitItemsData(app.Services);

            // Allow requests from the Blazor wasm
            app.UseCors(cors => cors.WithOrigins(
                builder.Configuration["Apps:BlazorWasm:Origin"]
            ).AllowAnyMethod().AllowAnyHeader());

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseGrpcWeb();
            app.MapGrpcService<ManufacturingDataService>().EnableGrpcWeb();
            app.Run();
        }
    }
}