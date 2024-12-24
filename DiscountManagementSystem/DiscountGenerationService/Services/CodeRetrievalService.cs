using DiscountGenerationService.Configurations;
using DiscountGenerationService.Data.DbContexts;
using DiscountGenerationService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DiscountGenerationService.Services
{
    public class CodeRetrievalService
    {
        // Assume we have a method to get the correct shard context based on clientId
        private readonly IDiscountShardedDbContextFactory _discount_sharded_context_factory;
        private readonly IUserRedirectionDbContextFactory _user_redirection_context_factory;
        private readonly string _shard_alphabet;

        private char GetOrAssignShard(Guid user_id)
        {
            UserToShardMapping entry;

            using (var context = _user_redirection_context_factory.CreateUserToShardDbContext())
            {
                entry = context.UserToShardMappings.Where(x => x.UserId.Equals(user_id)).FirstOrDefault();

                if (entry == null)
                {
                    Random random = new Random();
                    int random_index = random.Next(_shard_alphabet.Length);
                    entry = new UserToShardMapping() { UserId = user_id, ShardLabel = _shard_alphabet[random_index] };
                    context.UserToShardMappings.Add(entry);
                    context.SaveChanges();
                }
            }
            return entry.ShardLabel;
        }
        public CodeRetrievalService(
            IOptions<GlobalSettings> settings,
            IDiscountShardedDbContextFactory shard_context_factory, 
            IUserRedirectionDbContextFactory client_shard_mapping
        )
        {
            _discount_sharded_context_factory = shard_context_factory;
            _user_redirection_context_factory = client_shard_mapping;
            _shard_alphabet = settings.Value.ShardAlphabet;
        }

        async public Task<List<string>> GetDiscountCodesAsync(Guid user_id, uint count, ushort length)
        {
            using (var context = _discount_sharded_context_factory.CreateShardDbContext(GetOrAssignShard(user_id)))
            {
                // TRANSACTION START
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    if (length == 7)
                    {
                        var codes = await context.AvailableDiscounts7
                            .OrderBy(c => Guid.NewGuid())
                            .Take((int)count)
                            .ToListAsync();

                        context.AvailableDiscounts7.RemoveRange(codes);
                        context.EmittedDiscounts7.AddRange(codes.Select(c => new EmittedDiscount7 { Code = c.Code, CreationTime = c.CreationTime }));
                        await context.SaveChangesAsync();

                        await transaction.CommitAsync();
                        return codes.Select(c => c.Code).ToList();
                    }
                    else if (length == 8)
                    {
                        var codes = await context.AvailableDiscounts8
                            .OrderBy(c => Guid.NewGuid())
                            .Take((int)count)
                            .ToListAsync();

                        context.AvailableDiscounts8.RemoveRange(codes);
                        context.EmittedDiscounts8.AddRange(codes.Select(c => new EmittedDiscount8 { Code = c.Code, CreationTime = c.CreationTime }));
                        await context.SaveChangesAsync();

                        await transaction.CommitAsync();
                        return codes.Select(c => c.Code).ToList();
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
