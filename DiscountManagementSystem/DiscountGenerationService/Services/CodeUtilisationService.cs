using DiscountGenerationService.Configurations;
using DiscountGenerationService.Data.DbContexts;
using DiscountGenerationService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DiscountGenerationService.Services
{
    public class CodeUtilisationlService
    {
        // Assume we have a method to get the correct shard context based on clientId
        private readonly IDiscountShardedDbContextFactory _discount_sharded_context_factory;
        private readonly string _shard_alphabet;

        
        public CodeUtilisationlService(
            IOptions<GlobalSettings> settings,
            IDiscountShardedDbContextFactory shard_context_factory
        )
        {
            _discount_sharded_context_factory = shard_context_factory;
            _shard_alphabet = settings.Value.ShardAlphabet;
        }

        async public Task<bool> UseDiscountCodeAsync(string code)
        {
            using (var context = _discount_sharded_context_factory.CreateShardDbContext(code[0]))
            {
                // TRANSACTION START
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    if (code.Length == 7)
                    {
                        var confirmed_code = await context.EmittedDiscounts7.SingleOrDefaultAsync(x => x.Code.Equals(code));
                        
                        if (confirmed_code != null)
                        {
                            context.EmittedDiscounts7.Remove(confirmed_code);
                            context.UtilisedDiscounts7.Add(new UtilisedDiscount7 { Code = confirmed_code.Code, CreationTime = confirmed_code.CreationTime });
                            await context.SaveChangesAsync();

                            await transaction.CommitAsync();
                            return true;
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }                        
                    }
                    else if (code.Length == 8)
                    {
                        var confirmed_code = await context.EmittedDiscounts8.SingleOrDefaultAsync(x => x.Code.Equals(code));

                        if (confirmed_code != null)
                        {
                            context.EmittedDiscounts8.Remove(confirmed_code);
                            context.UtilisedDiscounts8.Add(new UtilisedDiscount8 { Code = confirmed_code.Code, CreationTime = confirmed_code.CreationTime });
                            await context.SaveChangesAsync();

                            await transaction.CommitAsync();
                            return true;
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Length must be 7 or 8");
                    }
                }
                // TRANSACTION END
            }
        }
    }

}
