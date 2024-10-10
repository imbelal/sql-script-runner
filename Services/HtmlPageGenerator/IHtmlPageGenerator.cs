using System.Collections.Generic;

namespace SqlScriptRunner.Services.HtmlPageGenerator;

public interface IHtmlPageGenerator
{
    string CreateHtmlPage(Dictionary<string, string> sqlScripts);
}