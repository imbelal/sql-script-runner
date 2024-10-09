namespace SqlScriptRunner.Services.DatabaseQueryExecutor;

public interface IScriptExecutionDeterminerService
{
    bool ShouldExecuteScript(string scriptFileName);
}