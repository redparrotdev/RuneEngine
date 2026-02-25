using RuneEngine.Interfaces;
using RuneEngine.Models;
using RuneEngine.Signals;
using System.Collections.Concurrent;

namespace RuneEngine;

public class RuneWorkflowRunner
{
    private readonly IRuneRegistry _registry;
    private readonly IServiceProvider? _serviceProvider;

    public RuneWorkflowRunner(
        IRuneRegistry registry
        , IServiceProvider? serviceProvider)
    {
        _registry = registry;
        _serviceProvider = serviceProvider;
    }

    public async Task<RuneWorkflowExecutionResult> Run(
        RuneWorkflow workflow
        , IReadOnlyDictionary<string, object?>? initialData = null
        , CancellationToken cancellationToken = default)
    {
        var result = new RuneWorkflowExecutionResult();

        foreach (var runeDefinition in GetRunesTopologicalOrder(workflow))
        {
            var output = await ExecuteRune(runeDefinition, result, cancellationToken, initialData);

            result[runeDefinition.Id] = output;
        }

        return result;
    }

    public async Task<RuneWorkflowExecutionResult> RunInParallel(
        RuneWorkflow workflow
        , IReadOnlyDictionary<string, object?>? initialData = null
        , CancellationToken cancellationToken = default)
    {
        var executionTasks = new ConcurrentDictionary<string, Task<RuneExecutionResult>>();

        foreach (var runeDefinition in GetRunesTopologicalOrder(workflow))
        {
            executionTasks[runeDefinition.Id] = RunRuneInParallel(
                runeDefinition
                , executionTasks
                , cancellationToken
                , initialData);
        }

        await Task.WhenAll(executionTasks.Values);

        var result = new RuneWorkflowExecutionResult();
        foreach (var (runeId, executionTask) in executionTasks)
        {
            result[runeId] = executionTask.Result;
        }

        return result;
    }

    private async Task<RuneExecutionResult> RunRuneInParallel(
        RuneDefinition runeDefinition
        , ConcurrentDictionary<string, Task<RuneExecutionResult>> executionTasks
        , CancellationToken cancellationToken
        , IReadOnlyDictionary<string, object?>? initialData = null)
    {
        var requiredInputs = runeDefinition.Inputs.Values
            .OfType<ConnectionBinding>()
            .DistinctBy(c => c.RuneId);

        foreach (var runeId in requiredInputs.Select(ri => ri.RuneId))
        {
            if (executionTasks.ContainsKey(runeId)) continue;

            throw new InvalidOperationException(
                $"Output from source rune with ID '{runeId}' not scheduled.");
        }

        var dependentRunesExecutionResultsTasksMap = requiredInputs
            .ToDictionary(
                c => c.RuneId
                , c => executionTasks[c.RuneId]);

        await Task.WhenAll(dependentRunesExecutionResultsTasksMap.Values);

        var workflowLikeResult = new RuneWorkflowExecutionResult();

        foreach (var (runeId, executionResultTask) in dependentRunesExecutionResultsTasksMap)
        {
            var executionResult = await executionResultTask;
            workflowLikeResult[runeId] = executionResult;
        }

        return await ExecuteRune(runeDefinition, workflowLikeResult, cancellationToken, initialData);
    }

    private async ValueTask<RuneExecutionResult> ExecuteRune(
        RuneDefinition runeDefinition
        , RuneWorkflowExecutionResult results
        , CancellationToken cancellationToken
        , IReadOnlyDictionary<string, object?>? initialData = null)
    {
        var inputs = ResolveInputs(runeDefinition, results);
        if (initialData is not null)
        {
            inputs = inputs.Concat(initialData).ToDictionary();
        }

        var signals = ResolveSignals(runeDefinition, results);

        var context = new RuneExecutionContext
        {
            RuneId = runeDefinition.Id,
            Inputs = inputs,
            Signals = signals,
            Services = _serviceProvider,
            CancellationToken = cancellationToken
        };

        var rune = _registry.Resolve(runeDefinition.Name);

        var beforeExecuteResult = await rune.BeforeExecuteAsync(context);
        if (beforeExecuteResult is not null)
        {
            return beforeExecuteResult;
        }

        return await rune.ExecuteAsync(context);
    }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
    private Dictionary<string, object?> ResolveInputs(
        RuneDefinition runeDefinition
        , RuneWorkflowExecutionResult results)
    {
        var runeDescription = _registry.Resolve(runeDefinition.Name).Description;

        var resolvedInputs = new Dictionary<string, object?>(runeDefinition.Inputs.Count);

        foreach (var inputPort in runeDescription.Inputs)
        {
            if (runeDefinition.Inputs.TryGetValue(inputPort.Name, out var inputBidning))
            {
                switch (inputBidning)
                {
                    case ConstantBinding constantBinding:
                        resolvedInputs[inputPort.Name] = constantBinding.Value;
                        break;
                    case ConnectionBinding connectionBinding:
                        if (!results.TryGetValue(connectionBinding.RuneId, out var sourceRuneOutput))
                        {
                            throw new InvalidOperationException(
                                $"Output from source rune with ID '{connectionBinding.RuneId}' not found.");
                        }
                        if (!sourceRuneOutput.Outputs.TryGetValue(connectionBinding.OutputName, out var value))
                        {
                            throw new InvalidOperationException(
                                $"Output '{connectionBinding.OutputName}' from source rune with ID " +
                                $"'{connectionBinding.RuneId}' not found.");
                        }

                        resolvedInputs[inputPort.Name] = value;
                        break;
                    default:
                        throw new NotSupportedException(
                            $"Unsupported input binding type: {inputBidning.GetType().Name}");
                }
            }
            else
            {
                if (inputPort.DefaultValue is not null)
                {
                    resolvedInputs[inputPort.Name] = inputPort.DefaultValue;
                }
                else if (!inputPort.Required)
                {
                    resolvedInputs[inputPort.Name] = null;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Required input '{inputPort.Name}' for rune '{runeDefinition.Name}' " +
                        $"is not provided and has no default value.");
                }
            }

            var resolvedValue = resolvedInputs[inputPort.Name];
            if (resolvedValue != null && !inputPort.ValueType.IsInstanceOfType(resolvedValue))
            {
                throw new InvalidOperationException(
                    $"Input '{inputPort.Name}' for rune '{runeDefinition.Name}' expects a value of type " +
                    $"'{inputPort.ValueType.FullName}', but got a value of type '{resolvedValue.GetType().FullName}'.");
            }
        }

        return resolvedInputs;
    }

    private Dictionary<string, ISignal> ResolveSignals(
        RuneDefinition runeDefinition
        , RuneWorkflowExecutionResult results)
    {
        var runeDescription = _registry.Resolve(runeDefinition.Name).Description;

        var resolvedSignals = new Dictionary<string, ISignal>(runeDescription.Inputs.Count);

        foreach (var inputPortName in runeDescription.Inputs.Select(port => port.Name))
        {
            if (!runeDefinition.Inputs.TryGetValue(inputPortName, out var inputBinding))
            {
                continue;
            }

            if (inputBinding is ConnectionBinding connectionBinding)
            {
                if (!results.TryGetValue(connectionBinding.RuneId, out var runeExecutionResult))
                {
                    continue;
                }

                if (!runeExecutionResult.Signals.TryGetValue(connectionBinding.OutputName, out var signal))
                {
                    continue;
                }

                resolvedSignals[inputPortName] = signal;
            }
        }

        return resolvedSignals;
    }

    private static List<RuneDefinition> GetRunesTopologicalOrder(RuneWorkflow workflow)
    {
        var runesById = workflow.Runes.ToDictionary(r => r.Id);

        var incoming = workflow.Runes.ToDictionary(
            r => r.Id
            , _ => new HashSet<string>());

        var outgoing = workflow.Runes.ToDictionary(
            r => r.Id
            , _ => new HashSet<string>());

        foreach (var rune in workflow.Runes)
        {
            foreach (var binding in rune.Inputs.Values)
            {
                if (binding is ConstantBinding) continue;

                if (binding is ConnectionBinding connectionBinding)
                {
                    if (!runesById.ContainsKey(connectionBinding.RuneId))
                    {
                        throw new InvalidOperationException(
                            $"Source rune with ID '{connectionBinding.RuneId}' not found.");
                    }

                    incoming[rune.Id].Add(connectionBinding.RuneId);
                    outgoing[connectionBinding.RuneId].Add(rune.Id);
                }
            }
        }

        var result = new List<RuneDefinition>(workflow.Runes.Count);

        var runesWithoutDependenciesQueue = new Queue<string>(
            incoming.Where(kv => kv.Value.Count == 0).Select(kv => kv.Key));

        while (runesWithoutDependenciesQueue.Count > 0)
        {
            var currentId = runesWithoutDependenciesQueue.Dequeue();
            result.Add(runesById[currentId]);
            foreach (var dependentId in outgoing[currentId])
            {
                incoming[dependentId].Remove(currentId);
                if (incoming[dependentId].Count == 0)
                {
                    runesWithoutDependenciesQueue.Enqueue(dependentId);
                }
            }
        }

        if (result.Count != workflow.Runes.Count)
        {
            var cycles = incoming
                .Where(kv => kv.Value.Count > 0)
                .Select(kv => kv.Key);

            throw new InvalidOperationException(
                $"Cyclic dependency detected among runes: {string.Join(", ", cycles)}");
        }

        return result;
    }
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
}
