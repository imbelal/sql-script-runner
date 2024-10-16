﻿using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.BlobStorage
{
    public interface IBlobStorage
    {
        /// <summary>
        /// Method to read scripts from given blob container.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Dictionary<string, string>> ReadAllSqlScriptsAsync(string containerName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method to read single sql script from give blob container and blob name.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> ReadSingleSqlScriptAsync(string containerName, string blobName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Download single blob.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BlobDownloadInfo> DownloadSingleBlobAsync(string containerName, string blobName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create and upload csv from sql script result.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="csvContainerPrefix"></param>
        /// <param name="blobName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UploadScriptsResultsToBlobAsync(List<string[]> results, string csvContainerPrefix, string blobName,
            CancellationToken cancellationToken = default);
    }
}
