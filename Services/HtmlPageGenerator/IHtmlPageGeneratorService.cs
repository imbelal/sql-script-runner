using System.Collections.Generic;

namespace SqlScriptRunner.Services.HtmlPageGenerator;

public interface IHtmlPageGeneratorService
{
    string CreateHtmlPage(Dictionary<string, string> sqlScripts);
}