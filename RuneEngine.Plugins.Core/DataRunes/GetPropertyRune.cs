using RuneEngine.Interfaces;
using RuneEngine.Models;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace RuneEngine.Plugins.Core.DataRunes;

internal sealed class GetPropertyRune : IRune
{

    #region Inputs

    private static readonly InputPort InputObject = new()
    {
        Name = "object",
        ValueType = typeof(object),
        Required = true
    };

    private static readonly InputPort InputPropertyName = new()
    {
        Name = "propertyName",
        ValueType = typeof(string),
        Required = true
    };

    #endregion

    #region Outputs

    private static readonly OutputPort OutputPropertyValue = new()
    {
        Name = "propertyValue",
        ValueType = typeof(object)
    };

    #endregion

    private static readonly RuneDescription RuneDescription = new()
    {
        Name = "core.get_property",
        Category = "Core",
        Inputs = [
            InputObject,
            InputPropertyName
        ],
        Outputs = [
            OutputPropertyValue
        ]
    };

    public RuneDescription Description => RuneDescription;

    public ValueTask<RuneExecutionResult> ExecuteAsync(RuneExecutionContext context)
    {
        var obj = context.Inputs[InputObject.Name]!;
        var propertyName = (string)context.Inputs[InputPropertyName.Name]!;

        var getter = PropertyHelper.GetGetter(obj.GetType(), propertyName);
        var propertyValue = getter(obj);

        var result = RuneExecutionResult.From((OutputPropertyValue.Name, propertyValue));

        return ValueTask.FromResult(result);
    }
}

file static class PropertyHelper
{
    public static readonly ConcurrentDictionary<(Type, string), Func<object, object?>> GettersCache = new();

    public static Func<object, object?> GetGetter(Type type, string propertyName)
    {
        return GettersCache.GetOrAdd((type, propertyName), key =>
        {
            var (t, p) = key;

            var propertyInfo = t.GetProperty(p, BindingFlags.Instance | BindingFlags.Public)
                ?? throw new InvalidOperationException($"Property '{p}' not found on type '{t.FullName}'.");

            if (!propertyInfo.CanRead)
            {
                throw new InvalidOperationException($"Property '{p}' on type '{t.FullName}' does not have a getter.");
            }

            var objParameter = Expression.Parameter(typeof(object), "obj");
            var typedParameter = Expression.Convert(objParameter, t);

            var propertyAccess = Expression.Property(typedParameter, propertyInfo);

            var castedResult = Expression.Convert(propertyAccess, typeof(object));

            var lambda = Expression.Lambda<Func<object, object?>>(castedResult, objParameter);

            return lambda.Compile();
        });
    }
}
