using BlazorInventory.Data.Request;
using BlazorInventory.Data.Response;

namespace BlazorInventory.Data.Repository
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<ItemsResponse> GetItems(ItemsRequest request);
    }
}
