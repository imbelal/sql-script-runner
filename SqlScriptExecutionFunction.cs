using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SqlScriptRunner.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace SqlScriptRunner
{
    public class SqlScriptExecutionFunction
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ISqlQueryExecutorService _sqlQueryExecutorService;
        private readonly IHtmlPageGeneratorService _htmlPageGeneratorService;
        private readonly ILogger<SqlScriptExecutionFunction> _logger;

        public SqlScriptExecutionFunction(IBlobStorageService blobStorageService, ISqlQueryExecutorService sqlQueryExecutorService, IHtmlPageGeneratorService htmlPageGeneratorService, ILogger<SqlScriptExecutionFunction> logger)
        {
            _blobStorageService = blobStorageService;
            _sqlQueryExecutorService = sqlQueryExecutorService;
            _htmlPageGeneratorService = htmlPageGeneratorService;
            _logger = logger;
        }

        [FunctionName("ExecuteAllScripts")]
        public async Task<IActionResult> ExecuteAllScript(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = "execute-scripts")] HttpRequest req)
        {
            _logger.LogInformation("Executing all scripts requested...");
            try
            {
                // Read all SQL scripts from Blob Storage
                var sqlScripts = await _blobStorageService.ReadAllSqlScriptsAsync("scripts");

                // Execute each SQL script and upload results
                foreach (var script in sqlScripts)
                {
                    var results = await _sqlQueryExecutorService.ExecuteSqlQueryAsync(script.Value, script.Key);
                    await _blobStorageService.UploadScriptsResultsToBlobAsync(results, script.Key.Replace(".sql", ".csv"));
                }

                string responseMessage = "All scripts executed and results uploaded successfully.";
                _logger.LogInformation(responseMessage);
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong!!", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        
        [FunctionName("ExecuteSingleScript")]
        public async Task<IActionResult> ExecuteScript(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = "execute-script/{filename}")] HttpRequest req, string fileName)
        {
            _logger.LogInformation($"Executing {fileName} requested...");
            try
            {
                // Read all SQL scripts from Blob Storage
                var sqlScript = await _blobStorageService.ReadSingleSqlScriptAsync("scripts", fileName);

                // Execute SQL script and upload results
                var results = await _sqlQueryExecutorService.ExecuteSqlQueryAsync(sqlScript, fileName);
                await _blobStorageService.UploadScriptsResultsToBlobAsync(results, fileName.Replace(".sql", ".csv"));

                string responseMessage = $"{fileName} has been executed and results uploaded successfully!";
                _logger.LogInformation(responseMessage);
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong!!", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        
        [FunctionName("ScriptList")]
        public async Task<IActionResult> GetScripts(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "scripts")] HttpRequest req)
        {
            _logger.LogInformation($"Script list requested...");
            try
            {
                // Read all SQL scripts from Blob Storage
                var sqlScripts = await _blobStorageService.ReadAllSqlScriptsAsync("scripts");

                // Create html page to visualize.
                string html = _htmlPageGeneratorService.CreateHtmlPage(sqlScripts);

                // Return the HTML content
                return new ContentResult
                {
                    Content = html,
                    ContentType = "text/html",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong!!", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        
        [FunctionName("DownloadSingleCsvFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "download/{fileName}")] HttpRequest req,
            string fileName,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Download request for blob: {fileName}");
            try
            {
                // Download the blob content to a temporary file
                BlobDownloadInfo downloadInfo =
                    await _blobStorageService.DownloadSingleBlobAsync(fileName, cancellationToken);

                // Set the content type and return the file as a download
                return new FileStreamResult(downloadInfo.Content, "application/octet-stream")
                {
                    FileDownloadName = fileName
                };
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading blob: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
