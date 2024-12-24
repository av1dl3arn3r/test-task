using DiscountGenerationService.Data.DbContexts;
using DiscountGenerationService.Data.Migrations;
using DiscountGenerationService.Helpers;
using DiscountGenerationService.BackgroundTasks;
using DiscountGenerationService.GrpcServices;
using DiscountGenerationService.Services;
using DiscountGenerationService.Configurations;
using Microsoft.Extensions.Options;

namespace DiscountGenerationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<GlobalSettings>(
                builder.Configuration.GetSection("GlobalSettings")
            );
            builder.Services.AddSingleton<IDiscountShardedDbContextFactory>(
                sp => new DiscountShardedDbContextFactory(
                    builder.Configuration.GetConnectionString("DiscountShardedDatabase")
                )
            );
            builder.Services.AddSingleton<IUserRedirectionDbContextFactory>(
                sp => new UserRedirectionDbContextFactory(
                    builder.Configuration.GetConnectionString("UserRedirectionDatabase")
                )
            );
            builder.Services.AddSingleton<ICodeGeneratorFactory>(
                sp => new CodeGeneratorFactory(
                    sp.GetRequiredService<IOptions<GlobalSettings>>(),
                    builder.Configuration.GetValue<string>("StateFileStorage")
                )
            );
            builder.Services.AddHostedService<BackgroundReplenishHostedService>(
                sp => new BackgroundReplenishHostedService(
                    sp.GetRequiredService<IOptions<GlobalSettings>>(),
                    sp.GetRequiredService<IDiscountShardedDbContextFactory>(),
                    sp.GetRequiredService<ICodeGeneratorFactory>(),
                    builder.Configuration.GetValue<string>("StateFileStorage")
                )
            );
            builder.Services.AddTransient<CodeRetrievalService>();
            builder.Services.AddTransient<CodeUtilisationlService>();
            builder.Services.AddGrpc();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                MigrationHelper.ApplyMigrationsForAllDiscountShards(scope.ServiceProvider);
                MigrationHelper.ApplyMigrationsForUserToShardMapper(scope.ServiceProvider);
                CodeGeneratorHelper.GenerateStatesForAllDiscountShards(scope.ServiceProvider);
            }
            app.MapGrpcService<DiscountService>();
            app.Run();
        }
    }
}
