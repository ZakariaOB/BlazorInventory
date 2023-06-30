using AutoMapper;
using BlazorInventory.Data;
using BlazorInventory.Data.Repository;
using BlazorInventory.Data.Request;
using BlazorInventory.Data.Response;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BlazorInventory.Server.Controllers
{
    [ApiController]
    public class ItemsApiController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;

        public ItemsApiController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet("api/items")]
        public IActionResult GetAll()
        {
            IEnumerable<Item> items = _itemRepository.GetAll();
            return Ok(items);
        }

        [HttpGet("api/items/request")]
        public async Task<IActionResult> GetItems(
            [FromQuery] string modifiedSinceTicks, 
            [FromQuery] string maxCount)
        {
            long mTicks = long.TryParse(modifiedSinceTicks, out long ticks) ? ticks : 0;
            int mCount = int.TryParse(maxCount, out int count) ? count : 0;

            ItemsRequest request = new()
            {
                MaxCount = mCount,
                ModifiedSinceTicks = mTicks
            };

            ItemsResponse response = await _itemRepository.GetItems(request);
            return Ok(response);
        }
    }
}
