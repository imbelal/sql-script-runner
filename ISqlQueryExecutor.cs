using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner
{
    public interface ISqlQueryExecutor
    {
        Task<List<string[]>> ExecuteSqlQueryAsync(string sqlQuery, string fileName, CancellationToken cancellationToken = default);
    }
}
