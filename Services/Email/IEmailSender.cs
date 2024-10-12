using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.Email
{
    public interface IEmailSender
    {
        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellation = default);
    }
}
