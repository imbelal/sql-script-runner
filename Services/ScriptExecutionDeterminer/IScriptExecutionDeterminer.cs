namespace SqlScriptRunner.Services.ScriptExecutionDeterminer;

public interface IScriptExecutionDeterminer
{
    bool ShouldExecuteScript(string scriptFileName);
}