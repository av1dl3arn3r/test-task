using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace DiscountGenerationService.Data.DbContexts
{
    public interface IDiscountShardedDbContextFactory
    {
        DiscountShardedDbContext CreateShardDbContext(char shard_label);
    }

    public class DiscountShardedDbContextFactory : IDiscountShardedDbContextFactory
    {
        private readonly string _connection_string_template;

        public DiscountShardedDbContextFactory(string connection_string_template)
        {
            _connection_string_template = connection_string_template;
        }

        public DiscountShardedDbContext CreateShardDbContext(char shard_label)
        {
            var options_builder = new DbContextOptionsBuilder<DiscountShardedDbContext>();
            options_builder
                .UseSqlServer(
                    _connection_string_template.Replace("{SHARD}", shard_label.ToString())
                )
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(
                    warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
                );

            return new DiscountShardedDbContext(options_builder.Options);
        }
    }
}
