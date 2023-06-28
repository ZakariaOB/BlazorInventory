using BlazorInventory.Data.Repository;
using BlazorInventory.Server.Data.Repository;

namespace BlazorInventory.Server.Config
{
    public static class ServiceInstaller
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemRevisedRepository, ItemRevisedRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISubGroupRepository, SubGroupRepository>();
        }
    }
}
