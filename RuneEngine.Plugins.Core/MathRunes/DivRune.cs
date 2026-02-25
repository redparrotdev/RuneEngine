using RuneEngine.Interfaces;
using RuneEngine.Models;

namespace RuneEngine.Plugins.Core.MathRunes;

internal sealed class DivRune : IRune
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

    private static readonly OutputPort OutputDiv = new()
    {
        Name = "div",
        ValueType = typeof(decimal)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.math.div",
        Category = "Core::Math",
        Inputs = [
            InputA
            , InputB
        ],
        Outputs = [
            OutputDiv
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var a = Convert.ToDecimal(context.Inputs[InputA.Name]);
        var b = Convert.ToDecimal(context.Inputs[InputB.Name]);

        if (b == 0)
        {
            throw new DivideByZeroException("Input 'b' cannot be zero for division.");
        }

        var div = a / b;

        var result = RuneExecutionResult.From((OutputDiv.Name, div));

        return ValueTask.FromResult(result);
    }
}
