using DiscountGenerationService.Configurations;
using DiscountGenerationService.Data.DbContexts;
using Microsoft.Extensions.Options;

namespace DiscountGenerationService.Helpers
{
    public static class CodeGeneratorHelper
    {
        public static void GenerateStatesForAllDiscountShards(IServiceProvider service_provider)
        {
            var code_generator_factory = service_provider.GetRequiredService<ICodeGeneratorFactory>();
            var settings = service_provider.GetRequiredService<IOptions<GlobalSettings>>();

            foreach (var shard_label in settings.Value.ShardAlphabet)
            {
                var code_generator_7 = code_generator_factory.CreateCodeGenerator(shard_label, 7);
                code_generator_7.CreateRandomSlots(shard_label.ToString());
                
                var code_generator_8 = code_generator_factory.CreateCodeGenerator(shard_label, 8);
                code_generator_8.CreateRandomSlots(shard_label.ToString());
            } 
        }
    }
}