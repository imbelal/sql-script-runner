using SqlScriptRunner.Services.Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlScriptRunner.Services.ReportGenerator
{
    public class ReportSender : IReportSender
    {
        private readonly IEmailSender _emailSender;
        private readonly string _toEmail;

        public ReportSender(IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _toEmail = Environment.GetEnvironmentVariable("TO_EMAIL");
        }

        public async Task SendReportAsync(Dictionary<string, bool> scriptsResult,
            CancellationToken cancellationToken = default)
        {
            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<html><head>");
            htmlBuilder.Append("<style>");
            htmlBuilder.Append(@"
                body {
                    font-family: Arial, sans-serif;
                    margin: 20px;
                }
                h1 {
                    color: #333;
                }
                table {
                    width: 100%;
                    border-collapse: collapse;
                    margin-top: 20px;
                }
                th, td {
                    padding: 12px;
                    text-align: left;
                    border: 1px solid #dddddd;
                }
                th {
                    background-color: #f2f2f2;
                    color: #333;
                }
                tr:nth-child(even) {
                    background-color: #f9f9f9;
                }
                tr:hover {
                    background-color: #f1f1f1;
                }
            ");
            htmlBuilder.Append("</style>");
            htmlBuilder.Append("</head><body>");
            htmlBuilder.Append("<h1>SQL Script Execution Report:</h1>");

            // Create the table and define the headers
            htmlBuilder.Append("<table>");
            htmlBuilder.Append("<thead><tr><th>Script Name</th><th>Execution Result</th></tr></thead>");
            htmlBuilder.Append("<tbody>");

            // Create a table row for each SQL script
            foreach (var script in scriptsResult)
            {
                string executionResult = script.Value ?
                    "<span style='color:green;'>Successfully executed</span>" :
                    "<span style='color:red;'>Failed</span>";

                htmlBuilder.Append("<tr>");
                htmlBuilder.Append($"<td>{script.Key}</td>");
                htmlBuilder.Append($"<td>{executionResult}</td>");
                htmlBuilder.Append("</tr>");
            }

            // Finish the table and HTML page
            htmlBuilder.Append("</tbody>");
            htmlBuilder.Append("</table>");
            htmlBuilder.Append("</body></html>");

            await _emailSender.SendEmailAsync(
                                    _toEmail,
                                    subject: $"Sql script execution report: {DateTime.Now:dd.MM.yyyy hh:mm:ss}",
                                    htmlBuilder.ToString(),
                                    cancellationToken);
        }
    }
}
