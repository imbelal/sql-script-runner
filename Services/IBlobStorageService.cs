using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services
{
    public interface IBlobStorageService
    {
        Task<Dictionary<string, string>> ReadAllSqlScriptsAsync(string containerName, CancellationToken cancellationToken = default);
        Task UploadResultsToBlobAsync(List<string[]> results, string blobName, CancellationToken cancellationToken = default);
    }
}
