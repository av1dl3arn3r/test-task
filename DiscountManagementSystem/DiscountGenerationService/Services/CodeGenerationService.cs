using DiscountGenerationService.Data.DbContexts;
using DiscountGenerationService.Data.Entities;
using DiscountGenerationService.Helpers;

namespace DiscountGenerationService.Services
{
    public class CodeGenerationService
    {
        private readonly DiscountShardedDbContext _context;
        private readonly CodeGenerator _code_generator;
        private readonly char _shard_letter;

        public CodeGenerationService(DiscountShardedDbContext context, CodeGenerator code_generator)
        {
            _context = context;
            _code_generator = code_generator;
        }

        async public Task<bool> PregenerateDiscountCodes7Async(int count = 100)
        {
            var codes = _code_generator.GenerateDiscountCodes(count);
            var entities = codes.Select(c => new AvailableDiscount7 { Code = c, CreationTime = DateTime.UtcNow });
            await _context.AvailableDiscounts7.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return true;
        }

        async public Task<bool> PregenerateDiscountCodes8Async(int count = 100)
        {
            var codes = _code_generator.GenerateDiscountCodes(count);
            var entities = codes.Select(c => new AvailableDiscount8 { Code = c, CreationTime = DateTime.UtcNow });
            await _context.AvailableDiscounts8.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
