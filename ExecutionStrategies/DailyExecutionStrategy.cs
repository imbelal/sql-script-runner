using System;

namespace SqlScriptRunner.ExecutionStrategies;

public class DailyExecutionStrategy : IScriptExecutionStrategy
{
    public bool ShouldExecute(string scriptFileName)
    {
        return scriptFileName.EndsWith("_daily.sql", StringComparison.OrdinalIgnoreCase);
    }
}