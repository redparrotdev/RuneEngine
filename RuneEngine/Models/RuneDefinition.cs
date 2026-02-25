namespace RuneEngine.Models;

public sealed class RuneDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }

    public required IReadOnlyDictionary<string, InputBinding> Inputs { get; init; }

    public IReadOnlyDictionary<string, object?> Metadata { get; init; }
        = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
