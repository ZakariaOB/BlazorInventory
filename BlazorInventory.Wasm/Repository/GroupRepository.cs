using BlazorInventory.Data.Models;
using BlazorInventory.Data.Repository;

namespace BlazorInventory.Data
{
    internal class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public GroupRepository(ClientSideDbContext dbContext) : base(dbContext)
        {
        }
    }
}
