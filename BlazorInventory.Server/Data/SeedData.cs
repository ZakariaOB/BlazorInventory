using BlazorInventory.Data;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;

namespace BlazorInventory.Server.Data
{
    public static class SeedData
    {
        public static void EnsureSeeded(IServiceProvider services, bool withDataIncrement = false)
        {
            var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (withDataIncrement)
            {
                if (dbContext.Database.CanConnect() && dbContext.Parts.Any())
                {
                    int partsCount = dbContext.Parts.Count();
                    AddPartsData(dbContext, partsCount);
                }
            }
            else if (dbContext.Database.EnsureCreated())
            {
                AddPartsData(dbContext, 0);
            }
        }

        public static void AddPartsData(ApplicationDbContext dbContext, int startCount = 0)
        {
            var dir = Path.GetDirectoryName(typeof(SeedData).Assembly.Location)!;
            using var fileReader = (TextReader)File.OpenText(Path.Combine(dir, "Data", "parts.csv"));
            using var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
            
            var count = startCount;
            bool takeSmallChunk = startCount > 0;

            while (csv.Read())
            {
                count++;
                
                Part part = new()
                {
                    PartId = count,
                    Category = csv.GetField<string>(0),
                    Subcategory = csv.GetField<string>(1),
                    Name = csv.GetField<string>(2),
                    Location = csv.GetField<string>(3),
                    PriceCents = (long)(csv.GetField<double>(4) * 100),
                    ModifiedTicks = count,
                    Stock = (int)Math.Round(50000 * Random.Shared.NextDouble() * Random.Shared.NextDouble() * Random.Shared.NextDouble())
                };
                dbContext.Parts.Add(part);
                
                if (count % 1000 == 0)
                {
                    Console.WriteLine($"Seeded item {count}...");
                }
                
                if (takeSmallChunk && ((count - startCount) == 1000))
                {
                    break;
                }
            }
            dbContext.SaveChanges();
        }
    }
}
