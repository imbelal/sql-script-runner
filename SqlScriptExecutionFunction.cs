using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SqlScriptRunner.ExceptionHandler;
using SqlScriptRunner.Services.BlobStorage;
using SqlScriptRunner.Services.DatabaseQueryExecutor;
using SqlScriptRunner.Services.HtmlPageGenerator;
using SqlScriptRunner.Services.ScriptExecutionDeterminer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner
{
    public class SqlScriptExecutionFunction
    {
        private const string ScriptsContainer = "scripts";
        private const string CsvContainerPrefix = "evaluations";

        private readonly IBlobStorage _blobStorage;
        private readonly ISqlQueryExecutor _sqlQueryExecutor;
        private readonly IHtmlPageGenerator _htmlPageGenerator;
        private readonly IScriptExecutionDeterminer _scriptExecutionDeterminer;
        private readonly ILogger<SqlScriptExecutionFunction> _logger;

        public SqlScriptExecutionFunction(
                IBlobStorage blobStorage,
                ISqlQueryExecutor sqlQueryExecutor,
                IScriptExecutionDeterminer scriptExecutionDeterminer,
                IHtmlPageGenerator htmlPageGenerator,
                ILogger<SqlScriptExecutionFunction> logger)
        {
            _blobStorage = blobStorage;
            _sqlQueryExecutor = sqlQueryExecutor;
            _scriptExecutionDeterminer = scriptExecutionDeterminer;
            _htmlPageGenerator = htmlPageGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Executes all SQL scripts on a timer trigger, runs at 1:10 AM daily.
        /// </summary>
        /// <param name="timer">Timer trigger info.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        [FunctionName("ExecuteAllScriptsTimer")]
        public async Task ExecuteAllScriptsTimer([TimerTrigger("27 21 * * *")] TimerInfo timerInfo) // Example: every night at 9: 27 PM.
        {
            _logger.LogInformation($"Timer trigger executed at: {DateTime.Now}");
            await ExecuteScriptsAsync();
        }

        /// <summary>
        /// HTTP-triggered function to execute all SQL scripts.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [FunctionName("ExecuteAllScripts")]
        public async Task<IActionResult> ExecuteAllScript(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = "execute-scripts")] HttpRequest req)
        {
            return await HandleFunctionExecutionAsync(req, async () =>
            {
                _logger.LogInformation("Executing all scripts requested...");

                string responseMessage = await ExecuteScriptsAsync();

                _logger.LogInformation(responseMessage);
                return new OkObjectResult(responseMessage);

            });
        }

        /// <summary>
        /// HTTP-triggered function to execute a single SQL script specified by file name.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="fileName">The name of the SQL script to execute.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [FunctionName("ExecuteSingleScript")]
        public async Task<IActionResult> ExecuteScript(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = "execute-script/{filename}")] HttpRequest req, string fileName)
        {
            return await HandleFunctionExecutionAsync(req, async () =>
            {
                _logger.LogInformation($"Executing {fileName} requested...");
                // Read the SQL script from Blob Storage
                var sqlScript = await _blobStorage.ReadSingleSqlScriptAsync(ScriptsContainer, fileName);
                // Execute the SQL script and upload results
                var results = await _sqlQueryExecutor.ExecuteSqlQueryAsync(sqlScript, fileName);
                await _blobStorage.UploadScriptsResultsToBlobAsync(results, CsvContainerPrefix, fileName);

                // Create response message and return.
                string responseMessage = $"{fileName} has been executed successfully and result has been uploaded!!";
                _logger.LogInformation(responseMessage);
                return new OkObjectResult(responseMessage);
            });
        }

        /// <summary>
        /// HTTP-triggered function to get a list of all available SQL scripts.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>An IActionResult that contains the HTML page with the script list.</returns>
        [FunctionName("ScriptList")]
        public async Task<IActionResult> GetScripts(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "scripts")] HttpRequest req)
        {
            return await HandleFunctionExecutionAsync(req, async () =>
            {
                _logger.LogInformation("Script list requested...");
                // Read all SQL scripts from Blob Storage
                var sqlScripts = await _blobStorage.ReadAllSqlScriptsAsync(ScriptsContainer);
                // Create HTML page to visualize.
                string html = _htmlPageGenerator.CreateHtmlPage(sqlScripts);

                // Return the HTML content
                return new ContentResult
                {
                    Content = html,
                    ContentType = "text/html",
                    StatusCode = StatusCodes.Status200OK
                };
            });
        }

        /// <summary>
        /// HTTP-triggered function to download a specific CSV result file.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="fileName">The name of the CSV file to download.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request if needed.</param>
        /// <returns>A FileStreamResult that represents the downloaded file.</returns>
        [FunctionName("DownloadSingleCsvFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "download/{fileName}")] HttpRequest req,
            string fileName,
            CancellationToken cancellationToken)
        {
            return await HandleFunctionExecutionAsync(req, async () =>
            {
                _logger.LogInformation($"Download request for blob: {fileName}");
                // Download the blob content
                BlobDownloadInfo downloadInfo = await _blobStorage.DownloadSingleBlobAsync(CsvContainerPrefix, fileName, cancellationToken);

                // Set the content type and return the file as a download
                return new FileStreamResult(downloadInfo.Content, "application/octet-stream")
                {
                    FileDownloadName = fileName
                };
            });
        }

        #region Private methods
        /// <summary>
        /// Executes all SQL scripts stored in the Blob Storage and uploads their results.
        /// </summary>
        /// <returns>A string message indicating the number of scripts executed.</returns>
        private async Task<string> ExecuteScriptsAsync()
        {
            // Read all SQL scripts from Blob Storage
            var sqlScripts = await _blobStorage.ReadAllSqlScriptsAsync(ScriptsContainer);

            int executedScriptsCount = 0;
            // Execute each SQL script and upload results
            foreach (var script in sqlScripts)
            {
                try
                {

                    if (_scriptExecutionDeterminer.ShouldExecuteScript(script.Key))
                    {
                        var results = await _sqlQueryExecutor.ExecuteSqlQueryAsync(script.Value, script.Key);
                        await _blobStorage.UploadScriptsResultsToBlobAsync(results, CsvContainerPrefix, script.Key);
                        executedScriptsCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Execution failed for {script.Key}", ex);
                }
            }

            // Create response message and return.
            string responseMessage = $"{executedScriptsCount} scripts executed successfully and results have been uploaded!!";
            _logger.LogInformation(responseMessage);
            return responseMessage;
        }

        /// <summary>
        /// Handles the execution of functions with a common exception handling mechanism.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="action">The function logic to execute.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        private async Task<IActionResult> HandleFunctionExecutionAsync(HttpRequest req, Func<Task<IActionResult>> action)
        {
            return await FunctionExceptionHandler.ExecuteAsync(action, req.HttpContext, _logger);
        }
        #endregion
    }
}
