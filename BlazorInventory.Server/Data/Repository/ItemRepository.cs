using BlazorInventory.Data;
using BlazorInventory.Data.Repository;
using BlazorInventory.Data.Request;
using BlazorInventory.Data.Response;
using Microsoft.EntityFrameworkCore;

namespace BlazorInventory.Server.Data.Repository
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        private ApplicationDbContext _dbContext;

        public ItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ItemsResponse> GetItems(ItemsRequest request)
        {
            if (request.MaxCount == 0 || request.ModifiedSinceTicks == 0)
            {
                return new ItemsResponse();
            }

            var modifiedItems = _dbContext.Items
                .OrderBy(p => p.Id)
                .Where(p => p.Id > request.ModifiedSinceTicks);

            ItemsResponse response = new();
            response.ModifiedCount = await modifiedItems.CountAsync();
            response.Items.AddRange(await modifiedItems.Take(request.MaxCount).ToListAsync());
            
            return response;
        }
    }
}
