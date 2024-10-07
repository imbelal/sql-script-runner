using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services
{
    public interface ISqlQueryExecutorService
    {
        /// <summary>
        /// Method to execute sql query.
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<string[]>> ExecuteSqlQueryAsync(string sqlQuery, string fileName, CancellationToken cancellationToken = default);
    }
}
