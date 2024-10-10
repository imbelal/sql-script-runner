using Microsoft.Extensions.Configuration;
using SqlScriptRunner.Services.DatabaseQueryHandlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlScriptRunner.Services.DatabaseQueryHandlerProvider;

public class DatabaseQueryHandlerProvider : IDatabaseQueryHandlerProvider
{
    private readonly IEnumerable<IDatabaseQueryHandler> _queryExecutors;
    private readonly IConfiguration _configuration;

    public DatabaseQueryHandlerProvider(IEnumerable<IDatabaseQueryHandler> queryExecutors, IConfiguration configuration)
    {
        _queryExecutors = queryExecutors;
        _configuration = configuration;
    }

    public IDatabaseQueryHandler CreateExecutor()
    {
        string databaseType = _configuration.GetValue<string>("DatabaseType");

        return databaseType switch
        {
            "SqlServer" => _queryExecutors.FirstOrDefault(q => q.GetType().Name == nameof(SqlServerQueryHandler)),
            "Postgres" => _queryExecutors.FirstOrDefault(q => q.GetType().Name == nameof(PostgresQueryHandler)),
            _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
        };
    }
}
