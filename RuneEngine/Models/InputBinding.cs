namespace RuneEngine.Models;

public abstract record InputBinding;

public sealed record ConstantBinding(object? Value) : InputBinding;

public sealed record ConnectionBinding(
    string RuneId,
    string OutputName)
    : InputBinding;
