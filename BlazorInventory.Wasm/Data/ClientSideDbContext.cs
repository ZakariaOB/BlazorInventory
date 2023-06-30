using BlazorInventory.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlazorInventory.Data
{
    internal partial class ClientSideDbContext : DbContext
    {
        public ClientSideDbContext(DbContextOptions<ClientSideDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                    Assembly.GetExecutingAssembly(),
                    t => t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<Item> Items { get; set; } = default!;

        public DbSet<ItemRevised> ItemsRevised { get; set; } = default!;

        public DbSet<Group> Groups { get; set; } = default!;

        public DbSet<SubGroup> SubGroups { get; set; } = default!;

        public void ClearTable<TEntity>() where TEntity : class
        {
            var entities = Set<TEntity>();
            entities.RemoveRange(entities);
            SaveChanges();
        }
    }
}
