using BlazorInventory.Data;
using BlazorInventory.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlazorInventory.Server.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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

        public void ClearTable<TEntity>() where TEntity : class
        {
            var entities = Set<TEntity>();
            entities.RemoveRange(entities);
            SaveChanges();
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<Item> Items { get; set; } = default!;

        public DbSet<ItemRevised> ItemsRevised { get; set; } = default!;

        public DbSet<Group> Groups { get; set; } = default!;

        public DbSet<SubGroup> SubGroups { get; set; } = default!;

    }
}
