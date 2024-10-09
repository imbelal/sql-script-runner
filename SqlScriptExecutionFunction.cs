using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SqlScriptRunner.ExceptionHandler;
using SqlScriptRunner.Services;
using SqlScriptRunner.Services.BlobStorage;
using SqlScriptRunner.Services.DatabaseQueryExecutor;
using SqlScriptRunner.Services.HtmlPageGenerator;

namespace SqlScriptRunner
{
    public class SqlScriptExecutionFunction
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ISqlQueryExecutorService _sqlQueryExecutorService;
        private readonly IHtmlPageGeneratorService _htmlPageGeneratorService;
        private readonly IScriptExecutionDeterminerService _scriptExecutionDeterminerService;
        private readonly ILogger<SqlScriptExecutionFunction> _logger;
        private const string ScriptsContainer = "scripts";
        private const string CsvContainerPrefix = "evaluations";

        public SqlScriptExecutionFunction(IBlobStorageService blobStorageService, ISqlQueryExecutorService sqlQueryExecutorService, IScriptExecutionDeterminerService scriptExecutionDeterminerService, IHtmlPageGeneratorService htmlPageGeneratorService, ILogger<SqlScriptExecutionFunction> logger)
        {
            _blobStorageService = blobStorageService;
            _sqlQueryExecutorService = sqlQueryExecutorService;
            _htmlPageGeneratorService = htmlPageGeneratorService;
            _scriptExecutionDeterminerService = scriptExecutionDeterminerService;
            _logger = logger;
        }

        [FunctionName("ExecuteAllScripts")]
        public async Task<IActionResult> ExecuteAllScript(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = "execute-scripts")] HttpRequest req)
        {
            return await FunctionExceptionHandler.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Executing all scripts requested...");
                // Read all SQL scripts from Blob Storage
                var sqlScripts = await _blobStorageService.ReadAllSqlScriptsAsync(ScriptsContainer);
                
                int executedScriptsCount = 0;
                // Execute each SQL script and upload results
                foreach (var script in sqlScripts)
                {
                    if (_scriptExecutionDeterminerService.ShouldExecuteScript(script.Key))
                    {
                        var results = await _sqlQueryExecutorService.ExecuteSqlQueryAsync(script.Value, script.Key);
                        await _blobStorageService.UploadScriptsResultsToBlobAsync(results, CsvContainerPrefix, script.Key);
                        executedScriptsCount++;
                    }
                }
                
                // Create response message and return.
                string responseMessage = $"{executedScriptsCount} scripts executed successfully and results have been uploaded!!";
                _logger.LogInformation(responseMessage);
                return new OkObjectResult(responseMessage);
                
            }, req.HttpContext, _logger);
        }
        
        [FunctionName("ExecuteSingleScript")]
        public async Task<IActionResult> ExecuteScript(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = "execute-script/{filename}")] HttpRequest req, string fileName)
        {
            return await FunctionExceptionHandler.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Executing {fileName} requested...");
                // Read the SQL script from Blob Storage
                var sqlScript = await _blobStorageService.ReadSingleSqlScriptAsync(ScriptsContainer, fileName);
                // Execute the SQL script and upload results
                var results = await _sqlQueryExecutorService.ExecuteSqlQueryAsync(sqlScript, fileName);
                await _blobStorageService.UploadScriptsResultsToBlobAsync(results, CsvContainerPrefix, fileName);

                // Create response message and return.
                string responseMessage = $"{fileName} has been executed successfully and result has been uploaded!!";
                _logger.LogInformation(responseMessage);
                return new OkObjectResult(responseMessage);
            }, req.HttpContext, _logger);
        }
        
        [FunctionName("ScriptList")]
        public async Task<IActionResult> GetScripts(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "scripts")] HttpRequest req)
        {
            return await FunctionExceptionHandler.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Script list requested...");
                // Read all SQL scripts from Blob Storage
                var sqlScripts = await _blobStorageService.ReadAllSqlScriptsAsync(ScriptsContainer);
                // Create HTML page to visualize.
                string html = _htmlPageGeneratorService.CreateHtmlPage(sqlScripts);
                
                // Return the HTML content
                return new ContentResult
                {
                    Content = html,
                    ContentType = "text/html",
                    StatusCode = StatusCodes.Status200OK
                };
            }, req.HttpContext, _logger);
        }
        
        [FunctionName("DownloadSingleCsvFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "download/{fileName}")] HttpRequest req,
            string fileName,
            CancellationToken cancellationToken)
        {
            return await FunctionExceptionHandler.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Download request for blob: {fileName}");
                // Download the blob content
                BlobDownloadInfo downloadInfo = await _blobStorageService.DownloadSingleBlobAsync(CsvContainerPrefix, fileName, cancellationToken);
                
                // Set the content type and return the file as a download
                return new FileStreamResult(downloadInfo.Content, "application/octet-stream")
                {
                    FileDownloadName = fileName
                };
            }, req.HttpContext, _logger);
        }
    }
}
