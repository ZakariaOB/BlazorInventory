using BlazorInventory.Data;
using BlazorInventory.Data.Repository;

namespace BlazorInventory.Server.Data.Repository
{
    public class ItemRevisedRepository : BaseRepository<ItemRevised>, IItemRevisedRepository
    {
        public ItemRevisedRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
