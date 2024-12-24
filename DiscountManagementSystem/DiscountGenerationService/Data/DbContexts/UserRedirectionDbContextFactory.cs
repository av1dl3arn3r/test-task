using Microsoft.EntityFrameworkCore;
using DiscountGenerationService.Data.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DiscountGenerationService.Data.DbContexts
{
    public interface IUserRedirectionDbContextFactory
    {
        UserRedirectionDbContext CreateUserToShardDbContext();
    }

    public class UserRedirectionDbContextFactory : IUserRedirectionDbContextFactory
    {
        private readonly string _connection_string_template;

        public UserRedirectionDbContextFactory(string connection_string_template)
        {
            _connection_string_template = connection_string_template;
        }

        public UserRedirectionDbContext CreateUserToShardDbContext()
        {
            var options_builder = new DbContextOptionsBuilder<UserRedirectionDbContext>();
            options_builder.UseSqlServer(_connection_string_template);
            options_builder.ConfigureWarnings(
                warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
            );

            return new UserRedirectionDbContext(options_builder.Options);
        }
    }
}
