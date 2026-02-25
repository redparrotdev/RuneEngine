using RuneEngine.Interfaces;
using RuneEngine.Models;

namespace RuneEngine.Plugins.Core.MathRunes;

internal sealed class MulRune : IRune
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

    private static readonly OutputPort OutputMul = new()
    {
        Name = "mul",
        ValueType = typeof(decimal)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.math.mul",
        Category = "Core::Math",
        Inputs = [
            InputA
            , InputB
        ],
        Outputs = [
            OutputMul
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var a = Convert.ToDecimal(context.Inputs[InputA.Name]);
        var b = Convert.ToDecimal(context.Inputs[InputB.Name]);

        var mul = a * b;

        var result = RuneExecutionResult.From((OutputMul.Name, mul));

        return ValueTask.FromResult(result);
    }
}
