using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SqlScriptRunner.ExecutionStrategies;
using SqlScriptRunner.Services;
using SqlScriptRunner.Services.BlobStorage;
using SqlScriptRunner.Services.DatabaseQueryExecutor;
using SqlScriptRunner.Services.HtmlPageGenerator;

[assembly: FunctionsStartup(typeof(SqlScriptRunner.Startup))]
namespace SqlScriptRunner
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register services
            builder.Services.AddTransient<IBlobStorageService, BlobStorageService>();
            builder.Services.AddTransient<ISqlQueryExecutorService, SqlQueryExecutorService>();
            builder.Services.AddSingleton<IHtmlPageGeneratorService, HtmlPageGeneratorService>();
            builder.Services.AddSingleton<IScriptExecutionStrategy, DailyExecutionStrategy>();
            builder.Services.AddSingleton<IScriptExecutionStrategy, WeeklyExecutionStrategy>();
            builder.Services.AddSingleton<IScriptExecutionStrategy, MonthlyExecutionStrategy>();
            builder.Services.AddSingleton<IScriptExecutionDeterminerService, ScriptExecutionDeterminerService>();
        }
    }
}
