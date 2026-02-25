using RuneEngine.Interfaces;
using RuneEngine.Models;
using RuneEngine.Signals;

namespace RuneEngine.Plugins.Core.LogicRunes;

internal sealed class AnyNonSkippedRune : IRune
{

    #region Inputs

    private static readonly InputPort InputA = new()
    {
        Name = "a",
        ValueType = typeof(object),
        Required = true
    };

    private static readonly InputPort InputB = new()
    {
        Name = "b",
        ValueType = typeof(object),
        Required = true
    };

    #endregion

    #region Outputs 

    private static readonly OutputPort OutputData = new()
    {
        Name = "data",
        ValueType = typeof(object)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.logic.any_non_skipped",
        Category = "Core::Logic",
        Inputs = [
            InputA
            , InputB
        ],
        Outputs = [
            OutputData
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult?> BeforeExecuteAsync(RuneExecutionContext context)
    {
        string[] inputPortsNames = [InputA.Name, InputB.Name];
        var bothPortsSkipped = inputPortsNames.All(portName => context.Signals.TryGetValue(portName, out var signal) && signal is SkipSignal);

        if (bothPortsSkipped)
        {
            throw new InvalidOperationException($"Both inputs of {Description.Name} are skipped, at least one must be non-skipped.");
        }

        return ValueTask.FromResult((RuneExecutionResult?)null);
    }

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var a = context.Inputs[InputA.Name];
        var b = context.Inputs[InputB.Name];

        var aIsSkipped = context.Signals.TryGetValue(InputA.Name, out var aSignal) && aSignal is SkipSignal;
        var bIsSkipped = context.Signals.TryGetValue(InputB.Name, out var bSignal) && bSignal is SkipSignal;

        object? resultData = a;

        if (aIsSkipped)
        {
            resultData = b;
        }

        if (bIsSkipped)
        {
            resultData = a;
        }

        var result = RuneExecutionResult.From((OutputData.Name, resultData));

        return ValueTask.FromResult(result);
    }
}
