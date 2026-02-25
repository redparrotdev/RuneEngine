using RuneEngine.Models;
using RuneEngine.Signals;

namespace RuneEngine.Extensions;

public static class RuneExecutionContextExtensions
{
    public static bool HasSkipSignals(this RuneExecutionContext context)
    {
        return context.Signals.Values.OfType<SkipSignal>().Any();
    }
}
