using RuneEngine.Interfaces;
using RuneEngine.Plugins.Core.DataRunes;
using RuneEngine.Plugins.Core.LogicRunes;
using RuneEngine.Plugins.Core.MathRunes;
using RuneEngine.Plugins.Core.PrimitivRunes;

namespace RuneEngine.Plugins.Core;

public sealed class RuneEngineCorePlugins : IRunePlugin
{
    public void Register(IRuneRegistry registry)
    {
        AddDataRunes(registry);
        AddMathRunes(registry);
        AddLogicRunes(registry);
        AddPrimitiveRunes(registry);
    }

    private static void AddDataRunes(IRuneRegistry registry)
    {
        registry.Register(new InputDataRune());
        registry.Register(new GetPropertyRune());
    }

    private static void AddMathRunes(IRuneRegistry registry)
    {
        registry.Register(new SumRune());
        registry.Register(new SubRune());
        registry.Register(new MulRune());
        registry.Register(new DivRune());
    }

    private static void AddLogicRunes(IRuneRegistry registry)
    {
        registry.Register(new CompareRune());
        registry.Register(new ConditionRune());
    }

    private static void AddPrimitiveRunes(IRuneRegistry registry)
    {
        registry.Register(new NullPrimitiveRune());
        registry.Register(new BoolPrimitiveRune());
        registry.Register(new IntPrimitiveRune());
        registry.Register(new DecimalPrimitiveRune());
        registry.Register(new StringPrimitiveRune());
    }
}
