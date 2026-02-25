namespace RuneEngine.Models;

public sealed class RuneDescription
{
    public required string Name { get; init; }

    public required IReadOnlyList<InputPort> Inputs { get; init; }
    public required IReadOnlyList<OutputPort> Outputs { get; init; }

    public string? DisplayName { get; init; }
    public string? Category { get; init; }
    public string? Description { get; init; }
    public Version Version { get; init; } = new Version(1, 0);
}
