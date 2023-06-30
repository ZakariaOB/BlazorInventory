using BlazorInventory.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorInventory.Server.Config
{
    public class SubGroupConfiguration : IEntityTypeConfiguration<SubGroup>
    {
        public void Configure(EntityTypeBuilder<SubGroup> builder)
        {
            builder.ToTable("SubGroup");

            builder.HasOne(p => p.Group)
                .WithMany()
                .HasForeignKey(p => p.GroupId);
        }
    }
}
