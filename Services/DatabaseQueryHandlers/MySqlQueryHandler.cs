using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.DatabaseQueryHandlers;

public class MySqlQueryHandler : IDatabaseQueryHandler
{
    public async Task<List<string[]>> ExecuteQueryAsync(string sqlQuery, CancellationToken cancellationToken = default)
    {
        string connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");
        var results = new List<string[]>();

        await using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await using (var command = new MySqlCommand(sqlQuery, connection))
            {
                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    // Read column names first
                    var columnNames = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }
                    results.Add(columnNames.ToArray());

                    // Read each row of data
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

        return results;
    }
}