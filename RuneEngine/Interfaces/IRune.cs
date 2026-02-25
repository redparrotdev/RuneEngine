using RuneEngine.Extensions;
using RuneEngine.Models;
using RuneEngine.Signals;

namespace RuneEngine.Interfaces;

public interface IRune
{
    RuneDescription Description { get; }

    ValueTask<RuneExecutionResult?> BeforeExecuteAsync(RuneExecutionContext context)
    {

        if (!context.HasSkipSignals())
        {
            return ValueTask.FromResult((RuneExecutionResult?)null);
        }

        Dictionary<string, object?> outputValues = new(Description.Outputs.Count);
        Dictionary<string, ISignal> outputSignals = new(Description.Outputs.Count);

        foreach (var outputPortName in Description.Outputs.Select(p => p.Name))
        {
            outputValues[outputPortName] = null;
            outputSignals[outputPortName] = new SkipSignal();
        }

        var result = new RuneExecutionResult
        {
            Outputs = outputValues, 
            Signals = outputSignals
        };

        return ValueTask.FromResult(result)!;
    }

    ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context);
}
