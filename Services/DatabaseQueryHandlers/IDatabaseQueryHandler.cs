using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.DatabaseQueryHandlers;

public interface IDatabaseQueryHandler
{
    /// <summary>
    /// Execute sql query.
    /// </summary>
    /// <param name="sqlQuery"></param>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<string[]>> ExecuteQueryAsync(string sqlQuery, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}