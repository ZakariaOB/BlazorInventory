using BlazorInventory.Data.Repository;

namespace BlazorInventory.Data
{
    internal class ItemRevisedRepository : BaseRepository<ItemRevised>, IItemRevisedRepository
    {
        public ItemRevisedRepository(ClientSideDbContext dbContext) : base(dbContext)
        {
        }
    }
}
