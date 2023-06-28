using AutoMapper;
using BlazorInventory.Data;
using BlazorInventory.Data.Repository;
using BlazorInventory.Data.Request;
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

        [HttpPost("api/items/request")]
        public IActionResult GetItems(ItemsRequest request)
        {
            var response = _itemRepository.GetItems(request);
            
            return Ok(response);
        }
    }
}
