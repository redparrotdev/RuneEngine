namespace RuneEngine.Models;

public sealed class RuneExecutionResult
{
    public required IReadOnlyDictionary<string, object?> Outputs { get; init; }

    public static RuneExecutionResult From(
        params (string Name, object? Value)[] values)
    {
        return new RuneExecutionResult
        {
            Outputs = values.ToDictionary(v => v.Name, v => v.Value)
        };
    }
}
