using Microsoft.EntityFrameworkCore;
using DiscountGenerationService.Data.Entities;

namespace DiscountGenerationService.Data.DbContexts
{
    public class UserRedirectionDbContext : DbContext
    {
        public DbSet<UserToShardMapping> UserToShardMappings { get; set; }

        public UserRedirectionDbContext(DbContextOptions<UserRedirectionDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder model_builder)
        {
            model_builder
                .Entity<UserToShardMapping>()
                .HasKey(c => c.UserId);

            base.OnModelCreating(model_builder);
        }
    }
}
