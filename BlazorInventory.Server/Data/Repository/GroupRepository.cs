using BlazorInventory.Data.Models;
using BlazorInventory.Data.Repository;

namespace BlazorInventory.Server.Data.Repository
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public GroupRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
