using BlazorInventory.Data;
using BlazorInventory.Data.Models;
using BlazorInventory.Data.Repository;
using CsvHelper;
using System;
using System.Globalization;

namespace BlazorInventory.Server.Data
{
    public static class SeedData
    {
        /*
        public static void EnsureSeeded(
            IServiceProvider services, 
            bool withDataIncrement = false)
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
        }*/
        /*
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
        */
        public static void InitItemsData(
            IServiceProvider services, 
            bool shouldInitialize = false)
        {

            var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            if (!dbContext.Database.CanConnect())
            {
                return;
            }

            if (dbContext.Items.Any() && !shouldInitialize)
            {
                return;
            }

            dbContext.ClearTable<Group>();
            dbContext.ClearTable<SubGroup>();
            dbContext.ClearTable<Item>();

            AddGroups(scope);
            AddSubGroups(scope);
            AddItems(scope);
        }

        public static void AddGroups(IServiceScope scope)
        {
            var dir = Path.GetDirectoryName(typeof(SeedData).Assembly.Location)!;
            using var fileReader = (TextReader)File.OpenText(Path.Combine(
                dir, 
                "Data", 
                "ItemsModel", 
                "groups.csv"));

            using var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
            var groupRepository = scope.ServiceProvider.GetRequiredService<IGroupRepository>();
            
            List<Group> groups = new ();
            while (csv.Read())
            {
                Group group = new()
                {
                    Id = csv.GetField<int>(0),
                    Name = csv.GetField<string>(1),
                    Description = csv.GetField<string>(2)
                };
                groups.Add(group);
            }
            groupRepository.AddRange(groups);
        }

        public static void AddSubGroups(IServiceScope scope)
        {
            var dir = Path.GetDirectoryName(typeof(SeedData).Assembly.Location)!;
            using var fileReader = (TextReader)File.OpenText(Path.Combine(
                dir,
                "Data",
                "ItemsModel",
                "subgroup.csv"));
            using var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
            var subGroupRepository = scope.ServiceProvider.GetRequiredService<ISubGroupRepository>();

            List<SubGroup> subGroups = new();
            while (csv.Read())
            {
                SubGroup subGroup = new()
                {
                    Id = csv.GetField<int>(0),
                    Name = csv.GetField<string>(1),
                    Description = csv.GetField<string>(2),
                    GroupId = csv.GetField<int>(3)
                };
                subGroups.Add(subGroup);
            }
            subGroupRepository.AddRange(subGroups);
        }

        public static void AddItems(IServiceScope scope)
        {
            var dir = Path.GetDirectoryName(typeof(SeedData).Assembly.Location)!;
            using var fileReader = (TextReader)File.OpenText(Path.Combine(
                dir,
                "Data",
                "ItemsModel",
                "items.csv"));

            using var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();

            List<Item> items = new();
            while (csv.Read())
            {
                Item item = new()
                {
                    Id = csv.GetField<int>(0),
                    Name = csv.GetField<string>(1),
                    Description = csv.GetField<string>(2),
                    Quantity = csv.GetField<int>(3),
                    Price = csv.GetField<decimal>(4),
                    Stock = csv.GetField<int>(5),
                    SalesCount = csv.GetField<int>(6),
                    ResponsibleIdentifier = csv.GetField<string>(7),
                    SubGroupId = csv.GetField<int>(8)
                };
                items.Add(item);
            }
            itemRepository.AddRange(items);

            // Generate items
            List<string> responsibles = itemRepository
                .GetAll()
                .Select(item => item.ResponsibleIdentifier)
                .ToList();
            List<int> subGroupIds = itemRepository
                .GetAll()
                .Select(item => item.SubGroupId)
                .ToList();
            List<Item> generatedItems = GenerateItems(
                responsibles.Count,
                100000,
                responsibles,
                subGroupIds);
            itemRepository.AddRange(generatedItems);
        }

        private static List<Item> GenerateItems(
            int startCount, 
            int count,
            List<string> responsibles,
            List<int> subGroupIds)
        {
            List<Item> items = new();

            for (int i = startCount + 1; i <= count; i++)
            {
                Item item = new()
                {
                    Id = i,
                    Name = GenerateRandomString(10),
                    Description = GenerateRandomString(20),
                    Quantity = new Random().Next(1, 100),
                    Price = (decimal)Math.Round(new Random().NextDouble() * (1000 - 1) + 1, 2),
                    Stock = new Random().Next(1, 1000),
                    SalesCount = new Random().Next(1, 100),
                    ResponsibleIdentifier = responsibles[new Random().Next(0, responsibles.Count)],
                    SubGroupId = subGroupIds[new Random().Next(0, subGroupIds.Count)]
                };
                items.Add(item);
            }
            return items;
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
