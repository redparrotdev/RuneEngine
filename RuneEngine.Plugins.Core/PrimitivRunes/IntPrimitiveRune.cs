using RuneEngine.Interfaces;
using RuneEngine.Models;

namespace RuneEngine.Plugins.Core.PrimitivRunes;

internal sealed class IntPrimitiveRune : IRune
{

    #region Inputs

    private static readonly InputPort InputValue = new()
    {
        Name = "value",
        ValueType = typeof(object),
        DefaultValue = 0
    };

    #endregion

    #region Outputs

    private static readonly OutputPort OutputValue = new()
    {
        Name = "value",
        ValueType = typeof(int)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.primitives.int",
        Category = "Core::Primitives",
        Inputs = [
            InputValue
        ],
        Outputs = [
            OutputValue
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var value = Convert.ToInt32(context.Inputs[InputValue.Name]!);
        var result = RuneExecutionResult.From((OutputValue.Name, value));

        return ValueTask.FromResult(result);
    }
}
