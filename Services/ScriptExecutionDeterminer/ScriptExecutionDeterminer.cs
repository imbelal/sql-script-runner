using SqlScriptRunner.ExecutionStrategies;
using System.Collections.Generic;

namespace SqlScriptRunner.Services.ScriptExecutionDeterminer;

public class ScriptExecutionDeterminer : IScriptExecutionDeterminer
{
    private readonly IEnumerable<IScriptExecutionStrategy> _strategies;

    public ScriptExecutionDeterminer(IEnumerable<IScriptExecutionStrategy> strategies)
    {
        _strategies = strategies;
    }

    public bool ShouldExecuteScript(string scriptFileName)
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.ShouldExecute(scriptFileName))
            {
                return true; // If any strategy returns true, execute the script
            }
        }

        return false; // No strategy matched, do not execute the script
    }
}