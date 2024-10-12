using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.DatabaseQueryHandlers;

public class SqlServerQueryHandler : IDatabaseQueryHandler
{
    public async Task<List<string[]>> ExecuteQueryAsync(string sqlQuery, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        var results = new List<string[]>();

        await using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await using (var command = new SqlCommand(sqlQuery, connection))
            {
                // Add parameters
                AddParams(command, parameters);
                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
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

        return results;
    }

    private static void AddParams(SqlCommand command, Dictionary<string, string> parameters)
    {
        // Add parameters to the command
        foreach (var param in parameters)
        {
            // Use AddWithValue to add parameters from the dictionary
            command.Parameters.AddWithValue($"@{param.Key}", param.Value);
        }
    }
}