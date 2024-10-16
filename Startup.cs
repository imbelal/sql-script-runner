﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SqlScriptRunner.ExecutionStrategies;
using SqlScriptRunner.Services.BlobStorage;
using SqlScriptRunner.Services.DatabaseQueryExecutor;
using SqlScriptRunner.Services.DatabaseQueryHandlerProvider;
using SqlScriptRunner.Services.DatabaseQueryHandlers;
using SqlScriptRunner.Services.Email;
using SqlScriptRunner.Services.HtmlPageGenerator;
using SqlScriptRunner.Services.ReportGenerator;
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
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<IReportSender, ReportSender>();

            // For SQL Server
            builder.Services.AddTransient<IDatabaseQueryHandler, SqlServerQueryHandler>();
            // For PostgreSQL
            builder.Services.AddTransient<IDatabaseQueryHandler, PostgresQueryHandler>();
            // For MySql
            builder.Services.AddTransient<IDatabaseQueryHandler, MySqlQueryHandler>();

            // Register the factory
            builder.Services.AddTransient<IDatabaseQueryHandlerProvider, DatabaseQueryHandlerProvider>();

            // Register the main service
            builder.Services.AddTransient<ISqlQueryExecutor, SqlQueryExecutor>();
        }
    }
}
