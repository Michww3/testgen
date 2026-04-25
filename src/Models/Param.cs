using System;

namespace Testgen.Models;

public sealed record Param
{
    public string Type { get; init; }
    public string Name { get; init; }
    public string Init { get; init; }
    public string HashExpr { get; init; }

    public Param(string name, string type, string init, string hashExpr)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type), "Param type must be provided");
        Name = name ?? throw new ArgumentNullException(nameof(name), "Param name must be provided");
        Init = init ?? throw new ArgumentNullException(nameof(init), "Param init must be provided");
        HashExpr = hashExpr ?? throw new ArgumentNullException(nameof(hashExpr), "Param hashExpr must be provided");
    }
}
