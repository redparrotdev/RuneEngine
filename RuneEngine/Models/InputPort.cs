namespace RuneEngine.Models;

public sealed class InputPort
{
    public required string Name { get; init; }
    public required Type ValueType { get; init; }
    public bool Required { get; init; }
    public object? DefaultValue { get; init; }

    public IReadOnlyDictionary<string, object?> Metadata { get; init; }
        = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
