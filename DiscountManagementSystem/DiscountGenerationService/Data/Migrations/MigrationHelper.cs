using DiscountGenerationService.Configurations;
using DiscountGenerationService.Data.DbContexts;
using DiscountGenerationService.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DiscountGenerationService.Data.Migrations
{
    public static class MigrationHelper
    {
        public static void ApplyMigrationsForAllDiscountShards(IServiceProvider service_provider)
        {
            var discount_sharded_context_factory = service_provider.GetRequiredService<IDiscountShardedDbContextFactory>();
            var settings = service_provider.GetRequiredService<IOptions<GlobalSettings>>();

            foreach (var shard_label in settings.Value.ShardAlphabet)
            {
                using (var db_context = discount_sharded_context_factory.CreateShardDbContext(shard_label))
                {
                    //db_context.Database.EnsureDeleted();
                    db_context.Database.EnsureCreated();
                    db_context.Database.Migrate();
                }
            }
        }

        public static void ApplyMigrationsForUserToShardMapper(IServiceProvider service_provider)
        {
            var user_redirection_context_factory = service_provider.GetRequiredService<IUserRedirectionDbContextFactory>();

            using (var db_context = user_redirection_context_factory.CreateUserToShardDbContext())
            {
                //db_context.Database.EnsureDeleted();
                db_context.Database.EnsureCreated();
                db_context.Database.Migrate();
            }
        }
    }
}
