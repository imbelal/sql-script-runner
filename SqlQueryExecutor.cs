using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner
{
    public class SqlQueryExecutor : ISqlQueryExecutor
    {
        private readonly ILogger _logger;
        public SqlQueryExecutor(ILogger<SqlQueryExecutor> logger)
        {
            _logger = logger;
        }

        public async Task<List<string[]>> ExecuteSqlQueryAsync(string sqlQuery, string fileName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Execution has started for ", fileName);
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            var results = new List<string[]>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        var columnNames = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columnNames.Add(reader.GetName(i));
                        }
                        results.Add(columnNames.ToArray());

                        while (await reader.ReadAsync(cancellationToken))
                        {
                            var row = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i].ToString();
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            _logger.LogInformation("Execution has finished for ", fileName);
            return results;
        }
    }
}
