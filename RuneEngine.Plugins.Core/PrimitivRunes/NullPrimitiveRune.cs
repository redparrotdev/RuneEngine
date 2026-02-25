using RuneEngine.Interfaces;
using RuneEngine.Models;

namespace RuneEngine.Plugins.Core.PrimitivRunes;

internal sealed class NullPrimitiveRune : IRune
{

    #region Outputs

    private static readonly OutputPort OutputValue = new()
    {
        Name = "value",
        ValueType = typeof(object)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.primitives.null",
        Category = "Core::Primitives",
        Outputs = [
            OutputValue
        ],
        Inputs = []
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var result = RuneExecutionResult.From((OutputValue.Name, null));
        return ValueTask.FromResult(result);
    }
}
