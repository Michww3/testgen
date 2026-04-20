using System;

namespace Testgen.Models;

public sealed record Param
{
    public string Type { get; init; }
    public string Name { get; init; }
    public string Init { get; init; }
    public string HashExpr { get; init; }

    public Param(string type, string name, string init, string hashExpr)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Init = init ?? throw new ArgumentNullException(nameof(init));
        HashExpr = hashExpr ?? throw new ArgumentNullException(nameof(hashExpr));
    }
}
