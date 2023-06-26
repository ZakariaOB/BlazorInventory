using BlazorInventory.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorInventory.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Part>().HasIndex(nameof(Part.ModifiedTicks), nameof(Part.PartId));
        }

        // Inventory
        public DbSet<Part> Parts { get; set; } = default!;
    }
}
