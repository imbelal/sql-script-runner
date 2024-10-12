using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.DatabaseQueryExecutor
{
    public interface ISqlQueryExecutor
    {
        /// <summary>
        /// Method to execute sql query.
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="fileName"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<string[]>> ExecuteSqlQueryAsync(string sqlQuery, string fileName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
    }
}
