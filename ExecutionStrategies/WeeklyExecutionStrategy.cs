using System;
using System.Linq;

namespace SqlScriptRunner.ExecutionStrategies;

public class WeeklyExecutionStrategy : IScriptExecutionStrategy
{
    public bool ShouldExecute(string scriptFileName)
    {
        if (!scriptFileName.Contains("_weekly_", StringComparison.OrdinalIgnoreCase))
            return false;

        var today = DateTime.Now.DayOfWeek;
        var dayPart = scriptFileName.Split('_').Last().Replace(".sql", "");
        
        if (Enum.TryParse<DayOfWeek>(dayPart, true, out var specifiedDay))
        {
            return specifiedDay == today; // Execute if today is the specified day
        }

        return false;
    }
}