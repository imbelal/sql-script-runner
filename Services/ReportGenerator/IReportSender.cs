using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.ReportGenerator
{
    public interface IReportSender
    {
        /// <summary>
        /// Send report based on scripts execution result.
        /// </summary>
        /// <param name="scriptsResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendReportAsync(Dictionary<string, bool> scriptsResult, CancellationToken cancellationToken = default);
    }
}
