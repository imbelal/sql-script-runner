using System;
using System.Collections.Generic;
using System.Text;

namespace SqlScriptRunner.Services;

public class HtmlPageGeneratorService : IHtmlPageGeneratorService
{
    public string CreateHtmlPage(Dictionary<string, string> sqlScripts)
    {
        // Start building the HTML page
        var htmlBuilder = new StringBuilder();
        htmlBuilder.Append("<html><head><style>");

        // Add some basic CSS for the table
        htmlBuilder.Append(@"body {
                                font-family: Arial, sans-serif;
                                margin: 40px;
                            }
                            h1 {
                                color: #333;
                            }
                            table {
                                width: 100%;
                                border-collapse: collapse;
                                margin-top: 20px;
                            }
                            table, th, td {
                                border: 1px solid #ddd;
                            }
                            th, td {
                                padding: 12px;
                                text-align: left;
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
                            a {
                                color: #007bff;
                                text-decoration: none;
                            }
                            a:hover {
                                text-decoration: underline;
                            }");

        htmlBuilder.Append("</style></head><body>");
        htmlBuilder.Append("<h1>Available SQL Scripts</h1>");
        htmlBuilder.Append($"<h3><a target=\"_blank\" href=\"/api/execute-scripts\">Execute all</a></h3>");

        // Create the table and define the headers
        htmlBuilder.Append("<table>");
        htmlBuilder.Append("<thead><tr><th>Script Name</th><th>Action</th></tr></thead>");
        htmlBuilder.Append("<tbody>");

        // Create a table row for each SQL script
        foreach (var script in sqlScripts)
        {
            // Assuming script has a Name or Key that you can use in the URL
            var scriptName = script.Key;
            var encodedScriptName = Uri.EscapeDataString(scriptName);
            var encodedCsvName = Uri.EscapeDataString(scriptName.Replace(".sql", ".csv"));

            // Add a row with a link to execute the script
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append($"<td>{scriptName}</td>");
            htmlBuilder.Append($"<td><a target=\"_blank\" href=\"/api/execute-script/{encodedScriptName}\">Execute</a> || ");
            htmlBuilder.Append($"<a href=\"/api/download/{encodedCsvName}\">Download</a></td>");
            htmlBuilder.Append("</tr>");
        }

        // Finish the table and HTML page
        htmlBuilder.Append("</tbody>");
        htmlBuilder.Append("</table>");
        htmlBuilder.Append("</body></html>");

        
        return htmlBuilder.ToString();
    }
}