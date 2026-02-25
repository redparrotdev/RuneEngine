using RuneEngine.Interfaces;
using RuneEngine.Models;
using RuneEngine.Signals;

namespace RuneEngine.Plugins.Core.LogicRunes;

internal sealed class BranchRune : IRune
{

    #region Inputs

    private static readonly InputPort InputData = new()
    {
        Name = "data",
        ValueType = typeof(object),
        Required = true
    };

    private static readonly InputPort InputCondition = new()
    {
        Name = "condition",
        ValueType = typeof(bool),
        Required = true
    };

    #endregion

    #region Outputs

    private static readonly OutputPort OutputOnTrue = new()
    {
        Name = "onTrue",
        ValueType = typeof(object)
    };

    private static readonly OutputPort OutputOnFalse = new()
    {
        Name = "onFalse",
        ValueType = typeof(object)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.logic.branch",
        Category = "Core::Logic",
        Inputs = [
            InputData
            , InputCondition
        ],
        Outputs = [
            OutputOnTrue
            , OutputOnFalse
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var data = context.Inputs[InputData.Name];
        var condition = (bool)context.Inputs[InputCondition.Name]!;

        var skipSignal = new SkipSignal();

        (string OuputPortName, string SkipPortName) = condition
            ? (OutputOnTrue.Name, OutputOnFalse.Name)
            : (OutputOnFalse.Name, OutputOnTrue.Name);

        var result = new RuneExecutionResult
        {
            Outputs = new Dictionary<string, object?>()
            {
                [OuputPortName] = data,
                [SkipPortName] = null
            },
            Signals = new Dictionary<string, ISignal>()
            {
                [SkipPortName] = skipSignal
            }
        };

        return ValueTask.FromResult(result);
    }
}
