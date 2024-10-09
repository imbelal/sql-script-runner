namespace SqlScriptRunner.Services;

public interface IScriptExecutionDeterminerService
{
    bool ShouldExecuteScript(string scriptFileName);
}