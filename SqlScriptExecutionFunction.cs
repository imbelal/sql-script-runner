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

        public SqlScriptExecutionFunction(IBlobStorageService blobStorageService, ISqlQueryExecutorService sqlQueryExecutorService)
        {
            _blobStorageService = blobStorageService;
            _sqlQueryExecutorService = sqlQueryExecutorService;
        }

        [FunctionName("ExecuteAllScripts")]
        public async Task<IActionResult> ExecuteAllScript(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = "execute-scripts")] HttpRequest req,
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
                    var results = await _sqlQueryExecutorService.ExecuteSqlQueryAsync(script.Value, script.Key);
                    await _blobStorageService.UploadScriptsResultsToBlobAsync(results, script.Key.Replace(".sql", ".csv"));
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
        
        [FunctionName("ExecuteSingleScript")]
        public async Task<IActionResult> ExecuteScript(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = "execute-script/{filename}")] HttpRequest req, string filename,
            ILogger log)
        {
            log.LogInformation("Function started.");
            string responseMessage;

            try
            {
                // Read all SQL scripts from Blob Storage
                var sqlScript = await _blobStorageService.ReadSingleSqlScriptAsync("scripts", filename);

                // Execute SQL script and upload results
                var results = await _sqlQueryExecutorService.ExecuteSqlQueryAsync(sqlScript, filename);
                await _blobStorageService.UploadScriptsResultsToBlobAsync(results, filename.Replace(".sql", ".csv"));

                responseMessage = "Script has been executed and results uploaded successfully.";
            }
            catch (Exception ex)
            {
                log.LogError("Something went wrong!!", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            log.LogInformation(responseMessage);
            return new OkObjectResult(responseMessage);
        }
        
        [FunctionName("ScriptList")]
        public async Task<IActionResult> GetScripts(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "scripts")] HttpRequest req,
            ILogger log)
        {
            try
            {
                // Read all SQL scripts from Blob Storage
                var sqlScripts = await _blobStorageService.ReadAllSqlScriptsAsync("scripts");

                // Create html page to visualize.
                string html = HtmlPageGeneratorService.CreateHtmlPage(sqlScripts);

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
                log.LogError("Something went wrong!!", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        
        [FunctionName("DownloadSingleCsvFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "download/{fileName}")] HttpRequest req,
            string fileName,
            ILogger log,
            CancellationToken cancellationToken)
        {
            log.LogInformation($"Download request for blob: {fileName}");
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
                log.LogWarning(ex.Message);
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError($"Error downloading blob: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
