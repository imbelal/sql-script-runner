using Microsoft.Extensions.Logging;
using SqlScriptRunner.Services.DatabaseQueryHandlerProvider;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.DatabaseQueryExecutor
{
    public class SqlQueryExecutor : ISqlQueryExecutor
    {
        private readonly ILogger _logger;
        private readonly IDatabaseQueryHandlerProvider _databaseQueryHandlerProvider;

        public SqlQueryExecutor(ILogger<SqlQueryExecutor> logger, IDatabaseQueryHandlerProvider databaseQueryHandlerProvider)
        {
            _logger = logger;
            _databaseQueryHandlerProvider = databaseQueryHandlerProvider;
        }

        public async Task<List<string[]>> ExecuteSqlQueryAsync(string sqlQuery, string fileName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Execution has started for {FileName}", fileName);

            var databaseQueryExecutor = _databaseQueryHandlerProvider.CreateExecutor();
            var results = await databaseQueryExecutor.ExecuteQueryAsync(sqlQuery, parameters, cancellationToken);

            _logger.LogInformation("Execution has finished for {FileName}", fileName);
            return results;
        }
    }

}
