using System.Collections.Generic;

namespace SqlScriptRunner.Services;

public interface IHtmlPageGeneratorService
{
    string CreateHtmlPage(Dictionary<string, string> sqlScripts);
}