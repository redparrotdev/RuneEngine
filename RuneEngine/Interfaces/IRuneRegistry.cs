using RuneEngine.Models;

namespace RuneEngine.Interfaces;

public interface IRuneRegistry
{
    void Register(IRune rune);
    public IRune Resolve(string name);
    IEnumerable<RuneDescription> GetAllRunesDescriptions();
}
