using RuneEngine.Interfaces;
using RuneEngine.Models;

namespace RuneEngine.Plugins.Core.MathRunes;

internal sealed class SubRune : IRune
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

    private static readonly OutputPort OutputSub = new()
    {
        Name = "sub",
        ValueType = typeof(decimal)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.math.sub",
        Category = "Core::Math",
        Inputs = [
            InputA
            , InputB
        ],
        Outputs = [
            OutputSub
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var a = Convert.ToDecimal(context.Inputs[InputA.Name]);
        var b = Convert.ToDecimal(context.Inputs[InputB.Name]);

        var sub = a - b;

        var result = RuneExecutionResult.From((OutputSub.Name, sub));

        return ValueTask.FromResult(result);
    }
}
