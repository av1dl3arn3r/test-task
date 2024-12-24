using DiscountGenerationService.Configurations;
using DiscountGenerationService.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;

namespace DiscountGenerationService.Helpers
{
    public interface ICodeGeneratorFactory
    {
        CodeGenerator CreateCodeGenerator(char shard_label, int length);
    }

    public class CodeGeneratorFactory : ICodeGeneratorFactory
    {
        private string _state_storage_template;
        private string _code_alphabet;

        public CodeGeneratorFactory(IOptions<GlobalSettings> settings, string state_storage_template)
        {
            _state_storage_template = state_storage_template;
            _code_alphabet = settings.Value.CodeAlphabet;
        }

        public CodeGenerator CreateCodeGenerator(char shard_label, int length)
        {
            string state_storage = _state_storage_template
                .Replace("{SHARD}", shard_label.ToString())
                .Replace("{LENGTH}", length.ToString());
            return new CodeGenerator(_code_alphabet, state_storage, length);
        }
    }
}
