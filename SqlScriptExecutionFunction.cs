using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SqlScriptRunner.Services;
using System;
using System.Threading.Tasks;

namespace SqlScriptRunner
{
    public class SqlScriptExecutionFunction
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ISqlQueryExecutor _sqlQueryExecutor;

        public SqlScriptExecutionFunction(IBlobStorageService blobStorageService, ISqlQueryExecutor sqlQueryExecutor)
        {
            _blobStorageService = blobStorageService;
            _sqlQueryExecutor = sqlQueryExecutor;
        }

        [FunctionName("execute-scripts")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Function started.");
            string responseMessage;

            try
            {
                // Read all SQL scripts from Blob Storage
                var sqlScripts = await _blobStorageService.ReadAllSqlScriptsAsync("scripts");

                // Execute each SQL script and upload results
                foreach (var script in sqlScripts)
                {
                    var results = await _sqlQueryExecutor.ExecuteSqlQueryAsync(script.Value, script.Key);
                    await _blobStorageService.UploadResultsToBlobAsync(results, script.Key.Replace(".sql", ".csv"));
                }

                responseMessage = "All scripts executed and results uploaded successfully.";
            }
            catch (Exception ex)
            {
                log.LogError("Something went wrong!!", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            log.LogInformation(responseMessage);
            return new OkObjectResult(responseMessage);
        }
    }
}
