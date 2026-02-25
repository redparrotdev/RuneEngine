using RuneEngine.Interfaces;
using RuneEngine.Models;

namespace RuneEngine.Plugins.Core.DataRunes;

internal sealed class InputDataRune : IRune
{

    #region Inputs

    private static readonly InputPort InputDataKey = new()
    {
        Name = "dataKey",
        ValueType = typeof(string),
        DefaultValue = "@global.input_data"
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
        Name = "core.input_data",
        Category = "Core",
        Outputs = [
            OutputData
        ],
        Inputs = [
            InputDataKey
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var dataKey = (string)context.Inputs[InputDataKey.Name]!;

        context.Inputs.TryGetValue(dataKey, out var data);

        var result = RuneExecutionResult.From((OutputData.Name, data));

        return ValueTask.FromResult(result);
    }
}
