namespace RuneEngine.Models;

public sealed class RuneExecutionContext
{
    public required string RuneId { get; init; }
    public required IReadOnlyDictionary<string, object?> Inputs { get; init; }
    public required CancellationToken CancellationToken { get; init; }

    public IServiceProvider? Services { get; init; }
}
