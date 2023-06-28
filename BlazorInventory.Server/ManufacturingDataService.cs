using BlazorInventory.Data;
using BlazorInventory.Server.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace BlazorInventory.Server
{
    public class ManufacturingDataService : ManufacturingData.ManufacturingDataBase
    {
        private readonly ApplicationDbContext db;

        public ManufacturingDataService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public override Task<DashboardReply> GetDashboardData(DashboardRequest request, ServerCallContext context)
        {
            return Task.FromResult(new DashboardReply
            {
                ProjectsBookedValue = 38_000_000,
                NextDeliveryDueInMs = (long)TimeSpan.FromHours(53).TotalMilliseconds,
                StaffOnSite = 441,
                FactoryUptimeMs = (long)TimeSpan.FromDays(152).TotalMilliseconds,
                ServicingTasksDue = 7,
                MachinesStopped = 3,
            });
        }

        public override async Task<PartsReply> GetParts(PartsRequest request, ServerCallContext context)
        {
            var modifiedParts = Enumerable.Empty<Part>();
                /*db.Parts
                .OrderBy(p => p.ModifiedTicks)
                .Where(p => p.ModifiedTicks > request.ModifiedSinceTicks);*/

            PartsReply reply = new ();
            /*
            reply.ModifiedCount = await modifiedParts.CountAsync();
            reply.Parts.AddRange(await modifiedParts.Take(request.MaxCount).ToListAsync());*/
            return reply;
        }
    }
}
