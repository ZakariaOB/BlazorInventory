using BlazorInventory.Data.Repository;
using BlazorInventory.Data.Request;
using BlazorInventory.Data.Response;

namespace BlazorInventory.Data
{
    internal class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public ItemRepository(ClientSideDbContext dbContext) : base(dbContext)
        {
        }

        public Task<ItemsResponse> GetItems(ItemsRequest request) => throw new NotImplementedException();
    }
}
