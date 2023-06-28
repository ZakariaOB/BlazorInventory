using BlazorInventory.Data.Models;
using BlazorInventory.Data.Repository;

namespace BlazorInventory.Server.Data.Repository
{
    public class SubGroupRepository : BaseRepository<SubGroup>, ISubGroupRepository
    {
        public SubGroupRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
