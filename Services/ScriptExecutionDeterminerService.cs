using System.Collections.Generic;
using SqlScriptRunner.ExecutionStrategies;

namespace SqlScriptRunner.Services;

public class ScriptExecutionDeterminerService : IScriptExecutionDeterminerService
{
    private readonly IEnumerable<IScriptExecutionStrategy> _strategies;

    public ScriptExecutionDeterminerService(IEnumerable<IScriptExecutionStrategy> strategies)
    {
        _strategies =  strategies;
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