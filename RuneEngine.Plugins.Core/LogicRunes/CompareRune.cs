using RuneEngine.Interfaces;
using RuneEngine.Models;
using System.Text.Json.Serialization;

namespace RuneEngine.Plugins.Core.LogicRunes;

internal sealed class CompareRune : IRune
{
    public enum ComparisonType
    {
        [JsonStringEnumMemberName("a=b")]
        Equals,

        [JsonStringEnumMemberName("a!=b")]
        NotEquals,

        [JsonStringEnumMemberName("a>b")]
        GreaterThan,

        [JsonStringEnumMemberName("a<b")]
        LessThan,

        [JsonStringEnumMemberName("a>=b")]
        GreaterThanOrEqual,

        [JsonStringEnumMemberName("a<=b")]
        LessThanOrEqual
    }

    #region Inputs

    private static readonly InputPort InputLeft = new()
    {
        Name = "left",
        ValueType = typeof(object),
        Required = true
    };

    private static readonly InputPort InputRight = new()
    {
        Name = "right",
        ValueType = typeof(object),
        Required = true
    };

    private static readonly InputPort InputComparison = new()
    {
        Name = "comparison",
        ValueType = typeof(ComparisonType),
        DefaultValue = ComparisonType.Equals
    };

    #endregion

    #region Outputs

    private static readonly OutputPort OutputIsEqual = new()
    {
        Name = "result",
        ValueType = typeof(bool)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.logic.compare",
        Category = "Core::Logic",
        Inputs = [
            InputLeft,
            InputRight,
            InputComparison
        ],
        Outputs = [
            OutputIsEqual
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var left = context.Inputs[InputLeft.Name]!;
        var right = context.Inputs[InputRight.Name]!;
        var comparison = (ComparisonType)context.Inputs[InputComparison.Name]!;

        var comparisonResult = comparison switch
        {
            ComparisonType.Equals => Equals(left, right),
            ComparisonType.NotEquals => !Equals(left, right),
            ComparisonType.GreaterThan => Comparer<object>.Default.Compare(left, right) > 0,
            ComparisonType.LessThan => Comparer<object>.Default.Compare(left, right) < 0,
            ComparisonType.GreaterThanOrEqual => Comparer<object>.Default.Compare(left, right) >= 0,
            ComparisonType.LessThanOrEqual => Comparer<object>.Default.Compare(left, right) <= 0,
            _ => throw new InvalidOperationException($"Unsupported comparison type: {comparison}")
        };

        var result = RuneExecutionResult.From((OutputIsEqual.Name, comparisonResult));

        return ValueTask.FromResult(result);
    }
}
