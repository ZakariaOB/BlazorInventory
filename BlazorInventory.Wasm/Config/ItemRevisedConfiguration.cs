using BlazorInventory.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BlazorInventory.Wasm.Config
{
    public class ItemRevisedConfiguration : IEntityTypeConfiguration<ItemRevised>
    {
        public void Configure(EntityTypeBuilder<ItemRevised> builder)
        {
            builder.ToTable("ItemRevised");
        }
    }
}
