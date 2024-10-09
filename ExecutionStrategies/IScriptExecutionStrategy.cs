namespace SqlScriptRunner.ExecutionStrategies;

public interface IScriptExecutionStrategy
{
    bool ShouldExecute(string scriptFileName);
}