namespace RuneEngine.Models;

public sealed class RuneWorkflow
{
    public required string Name { get; init; }

    public required IReadOnlyList<RuneDefinition> Runes { get; init; }

    public Version Version { get; init; } = new Version(1, 0);
    public IReadOnlyDictionary<string, object?> Metadata { get; init; }
        = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
