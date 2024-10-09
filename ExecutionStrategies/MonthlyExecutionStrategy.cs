using System;
using System.Linq;

namespace SqlScriptRunner.ExecutionStrategies;

public class MonthlyExecutionStrategy : IScriptExecutionStrategy
{
    public bool ShouldExecute(string scriptFileName)
    {
        if (!scriptFileName.Contains("_monthly_", StringComparison.OrdinalIgnoreCase))
            return false;

        var today = DateTime.Now.Day;
        var dayPart = scriptFileName.Split('_').Last().Replace(".sql", "");
        
        if (int.TryParse(dayPart, out var specifiedDay))
        {
            return specifiedDay == today; // Execute if today is the specified day of the month
        }

        return false;
    }
}