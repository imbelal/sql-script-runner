﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.BlobStorage
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger _logger;

        public BlobStorage(ILogger<BlobStorage> logger)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> ReadAllSqlScriptsAsync(string containerName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Reading all sql scripts...");
            var sqlScripts = new Dictionary<string, string>();
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(cancellationToken: cancellationToken))
            {
                if (!blobItem.Name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)) continue;
                BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                BlobDownloadInfo download = await blobClient.DownloadAsync(cancellationToken);

                using (var reader = new StreamReader(download.Content, Encoding.UTF8))
                {
                    string scriptContent = await reader.ReadToEndAsync();
                    sqlScripts.Add(blobItem.Name, scriptContent);
                }
            }
            _logger.LogInformation("Reading all sql scripts has been completed!");
            return sqlScripts;
        }

        public async Task<string> ReadSingleSqlScriptAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Reading {blobName}...");
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            BlobDownloadInfo download = await blobClient.DownloadAsync(cancellationToken);

            string scriptContent;
            using (var reader = new StreamReader(download.Content, Encoding.UTF8))
            {
                scriptContent = await reader.ReadToEndAsync();
            }
            _logger.LogInformation($"Reading {blobName} has been completed!");
            return scriptContent;
        }

        public async Task<BlobDownloadInfo> DownloadSingleBlobAsync(string containerName, string blobName,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Checking container...");
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!await containerClient.ExistsAsync())
            {
                _logger.LogInformation("Container does not exist!");
                return null;
            }

            _logger.LogInformation($"Checking the blob {blobName}...");
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            if (!await blobClient.ExistsAsync())
            {
                _logger.LogInformation("Blob does not exist!");
                return null;
            }
            _logger.LogInformation($"Downloading the blob {blobName}...");
            BlobDownloadInfo downloadInfo = await blobClient.DownloadAsync(cancellationToken);
            return downloadInfo;
        }

        public async Task UploadScriptsResultsToBlobAsync(List<string[]> results, string csvContainerPrefix, string blobName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Uploading has started for ", blobName);
            blobName = blobName.Replace(".sql", ".csv");
            string containerName = $"{csvContainerPrefix}-{DateTime.Now:dd-MM-yyyy}";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            using (var memoryStream = new MemoryStream())
            {
                await using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
                {
                    foreach (var row in results)
                    {
                        var line = string.Join(",", row);
                        await writer.WriteLineAsync(line);
                    }
                }

                memoryStream.Position = 0;
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(memoryStream, true, cancellationToken);
            }
            _logger.LogInformation("Uploading has finished for ", blobName);
        }
    }
}
