namespace RuneEngine.Models;

public sealed class OutputPort
{
    public required string Name { get; init; }
    public required Type ValueType { get; init; }
}
