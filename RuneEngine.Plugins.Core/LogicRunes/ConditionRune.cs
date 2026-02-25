using RuneEngine.Interfaces;
using RuneEngine.Models;

namespace RuneEngine.Plugins.Core.LogicRunes;

internal sealed class ConditionRune : IRune
{

    #region Inputs

    private static readonly InputPort InputCondition = new()
    {
        Name = "condition",
        ValueType = typeof(bool),
        Required = true
    };

    private static readonly InputPort InputTrueBranch = new()
    {
        Name = "onTrue",
        ValueType = typeof(object),
        Required = true
    };

    private static readonly InputPort InputFalseBranch = new()
    {
        Name = "onFalse",
        ValueType = typeof(object),
        Required = true
    };

    #endregion

    #region Outputs

    private static readonly OutputPort OutputResult = new()
    {
        Name = "result",
        ValueType = typeof(object)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.logic.condition",
        Category = "Core::Logic",
        Inputs = [
            InputCondition,
            InputTrueBranch,
            InputFalseBranch
        ],
        Outputs = [
            OutputResult
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var condition = (bool)context.Inputs[InputCondition.Name]!;
        var trueBranch = context.Inputs[InputTrueBranch.Name]!;
        var falseBranch = context.Inputs[InputFalseBranch.Name]!;

        var conditionResult = condition ? trueBranch : falseBranch;

        var result = RuneExecutionResult.From((OutputResult.Name, conditionResult));

        return ValueTask.FromResult(result);
    }
}
