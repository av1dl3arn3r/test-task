using Microsoft.EntityFrameworkCore;
using DiscountGenerationService.Data.Entities;

namespace DiscountGenerationService.Data.DbContexts
{
    public class DiscountShardedDbContext : DbContext
    {
        public DbSet<AvailableDiscount7> AvailableDiscounts7 { get; set; }
        public DbSet<EmittedDiscount7> EmittedDiscounts7 { get; set; }
        public DbSet<UtilisedDiscount7> UtilisedDiscounts7 { get; set; }
        public DbSet<AvailableDiscount8> AvailableDiscounts8 { get; set; }
        public DbSet<EmittedDiscount8> EmittedDiscounts8 { get; set; }
        public DbSet<UtilisedDiscount8> UtilisedDiscounts8 { get; set; }

        public DiscountShardedDbContext(DbContextOptions<DiscountShardedDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder model_builder)
        {
            model_builder
                .Entity<AvailableDiscount7>()
                .HasKey(c => c.Code);
            model_builder
                .Entity<EmittedDiscount7>()
                .HasKey(c => c.Code);
            model_builder
                .Entity<UtilisedDiscount7>()
                .HasKey(c => c.Code);
            model_builder
                .Entity<AvailableDiscount8>()
                .HasKey(c => c.Code);
            model_builder
                .Entity<EmittedDiscount8>()
                .HasKey(c => c.Code);
            model_builder
                .Entity<UtilisedDiscount8>()
                .HasKey(c => c.Code);

            base.OnModelCreating(model_builder);
        }
    }
}
