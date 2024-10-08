using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SqlScriptRunner.Services;

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
        }
    }
}
