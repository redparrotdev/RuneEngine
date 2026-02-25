using RuneEngine.Models;

namespace RuneEngine.Interfaces;

public interface IRune
{
    RuneDescription Description { get; }

    ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context);
}
