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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddGrpc();

            var app = builder.Build();
            SeedData.EnsureSeeded(app.Services, withDataIncrement: false);
            
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