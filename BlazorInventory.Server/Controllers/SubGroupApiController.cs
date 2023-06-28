using BlazorInventory.Data.Models;
using BlazorInventory.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BlazorInventory.Server.Controllers
{
    [ApiController]
    public class SubGroupApiController : ControllerBase
    {
        private readonly ISubGroupRepository _subGroupRepository;

        public SubGroupApiController(ISubGroupRepository subGroupRepository)
        {
            _subGroupRepository = subGroupRepository;
        }

        [HttpGet("api/subgroups")]
        public IActionResult GetAll()
        {
            IEnumerable<SubGroup> subGroups = _subGroupRepository.GetAll();
            return Ok(subGroups);
        }
    }
}
