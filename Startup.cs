using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SqlScriptRunner.ExecutionStrategies;
using SqlScriptRunner.Services.BlobStorage;
using SqlScriptRunner.Services.DatabaseQueryExecutor;
using SqlScriptRunner.Services.DatabaseQueryExecutorProvider;
using SqlScriptRunner.Services.DatabaseQueryHandlers;
using SqlScriptRunner.Services.HtmlPageGenerator;
using SqlScriptRunner.Services.ScriptExecutionDeterminer;

[assembly: FunctionsStartup(typeof(SqlScriptRunner.Startup))]
namespace SqlScriptRunner
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register services
            builder.Services.AddTransient<IBlobStorage, BlobStorage>();
            builder.Services.AddSingleton<IHtmlPageGenerator, HtmlPageGenerator>();
            builder.Services.AddSingleton<IScriptExecutionStrategy, DailyExecutionStrategy>();
            builder.Services.AddSingleton<IScriptExecutionStrategy, WeeklyExecutionStrategy>();
            builder.Services.AddSingleton<IScriptExecutionStrategy, MonthlyExecutionStrategy>();
            builder.Services.AddSingleton<IScriptExecutionDeterminer, ScriptExecutionDeterminer>();

            // For SQL Server
            builder.Services.AddTransient<IDatabaseQueryHandler, SqlServerQueryHandler>();

            // For PostgreSQL
            builder.Services.AddTransient<IDatabaseQueryHandler, PostgresQueryHandler>();

            // Register the factory
            builder.Services.AddTransient<IDatabaseQueryHandlerProvider, DatabaseQueryHandlerProvider>();

            // Register the main service
            builder.Services.AddTransient<ISqlQueryExecutor, SqlQueryExecutor>();
        }
    }
}
