using BlazorInventory.Data.Models;
using BlazorInventory.Data.Repository;

namespace BlazorInventory.Data
{
    internal class SubGroupRepository : BaseRepository<SubGroup>, ISubGroupRepository
    {
        public SubGroupRepository(ClientSideDbContext dbContext) : base(dbContext)
        {
        }
    }
}
