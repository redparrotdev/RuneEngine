using RuneEngine.Signals;

namespace RuneEngine.Models;

public sealed class RuneExecutionResult
{
    public required IReadOnlyDictionary<string, object?> Outputs { get; init; }

    public IReadOnlyDictionary<string, ISignal> Signals { get; init; } = new Dictionary<string, ISignal>(StringComparer.OrdinalIgnoreCase);

    public static RuneExecutionResult From(
        params (string Name, object? Value)[] values)
    {
        return new RuneExecutionResult
        {
            Outputs = values.ToDictionary(v => v.Name, v => v.Value)
        };
    }

    public static RuneExecutionResult From(
        (string Name, object? Value)[] values
        , (string Name, ISignal Signal)[] signals)
    {
        return new RuneExecutionResult
        {
            Outputs = values.ToDictionary(v => v.Name, v => v.Value),
            Signals = signals.ToDictionary(v => v.Name, v => v.Signal)
        };
    }
}
