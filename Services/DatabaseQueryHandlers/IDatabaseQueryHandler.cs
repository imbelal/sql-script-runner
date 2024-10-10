using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.DatabaseQueryHandlers;

public interface IDatabaseQueryHandler
{
    Task<List<string[]>> ExecuteQueryAsync(string sqlQuery, CancellationToken cancellationToken = default);
}