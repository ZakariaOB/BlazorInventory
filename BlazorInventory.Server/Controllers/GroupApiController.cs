using BlazorInventory.Data.Models;
using BlazorInventory.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BlazorInventory.Server.Controllers
{
    [ApiController]
    public class GroupApiController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;

        public GroupApiController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [HttpGet("api/groups")]
        public IActionResult GetAll()
        {
            IEnumerable<Group> groups = _groupRepository.GetAll();
            return Ok(groups);
        }
    }
}
