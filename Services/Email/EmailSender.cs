using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _username;
        private readonly string _password;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            // Read SMTP settings from environment variables
            _smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
            _username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            _password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellation = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_username, _username));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;

            // Set the body of the email
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls, cancellation).ConfigureAwait(false);
                await client.AuthenticateAsync(_username, _password, cancellation).ConfigureAwait(false);
                await client.SendAsync(message, cancellation).ConfigureAwait(false);
                await client.DisconnectAsync(true, cancellation).ConfigureAwait(false);
            }
        }
    }
}
