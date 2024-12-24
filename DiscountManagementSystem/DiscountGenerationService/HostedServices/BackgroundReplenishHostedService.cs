using DiscountGenerationService.Configurations;
using DiscountGenerationService.Data.DbContexts;
using DiscountGenerationService.Services;
using DiscountGenerationService.Helpers;
using Microsoft.Extensions.Options;

namespace DiscountGenerationService.BackgroundTasks
{
    public class BackgroundReplenishHostedService : BackgroundService
    {
        private readonly IDiscountShardedDbContextFactory _discount_sharded_context_factory;
        private readonly ICodeGeneratorFactory _code_generator_factory;
        private readonly string _storage;
        private readonly string _shard_alphabet;

        private const int MINIMAL_UPDATE_AMOUNT = 5000;
        private const int THRESHOLD_AMOUNT = 1000;

        public BackgroundReplenishHostedService(
            IOptions<GlobalSettings> settings,
            IDiscountShardedDbContextFactory discount_sharded_context_factory,
            ICodeGeneratorFactory code_generator_factory,
            string storage
        )
        {
            _discount_sharded_context_factory = discount_sharded_context_factory;
            _code_generator_factory = code_generator_factory;
            _storage = storage;
            _shard_alphabet = settings.Value.ShardAlphabet;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellation_token)
        {
            while (!cancellation_token.IsCancellationRequested)
            {
                foreach (var shard_letter in _shard_alphabet)
                {
                    using (var context = _discount_sharded_context_factory.CreateShardDbContext(shard_letter))
                    {
                        int available_7 = context.AvailableDiscounts7.Count();
                        if (available_7 < THRESHOLD_AMOUNT)
                        {
                            var svc = new CodeGenerationService(
                                context, 
                                _code_generator_factory.CreateCodeGenerator(shard_letter, 7)
                            );
                            await svc.PregenerateDiscountCodes7Async(MINIMAL_UPDATE_AMOUNT - available_7);
                        }

                        int available_8 = context.AvailableDiscounts8.Count();
                        if (available_8 < THRESHOLD_AMOUNT)
                        {
                            var svc = new CodeGenerationService(
                                context, 
                                _code_generator_factory.CreateCodeGenerator(shard_letter, 8)
                            );
                            await svc.PregenerateDiscountCodes8Async(MINIMAL_UPDATE_AMOUNT - available_8);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellation_token);
            }
        }
    }

}
