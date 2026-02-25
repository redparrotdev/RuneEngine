using RuneEngine.Interfaces;
using RuneEngine.Models;

namespace RuneEngine.Plugins.Core.PrimitivRunes;

internal sealed class BoolPrimitiveRune : IRune
{

    #region Inputs

    private static readonly InputPort InputValue = new()
    {
        Name = "value",
        ValueType = typeof(bool),
        DefaultValue = false
    };

    #endregion

    #region Outputs

    private static readonly OutputPort OutputValue = new()
    {
        Name = "value",
        ValueType = typeof(bool)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.primitives.bool",
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
        var value = (bool)context.Inputs[InputValue.Name]!;
        var result = RuneExecutionResult.From((OutputValue.Name, value));

        return ValueTask.FromResult(result);
    }
}
