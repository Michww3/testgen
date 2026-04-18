using System;

namespace TestGenerator;

public sealed record Param
{
    public string Type { get; }
    public string Name { get; }
    public string Init { get; }
    public string HashExpr { get; }

    public Param(string type, string name, string init, string hashExpr)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Init = init ?? throw new ArgumentNullException(nameof(init));
        HashExpr = hashExpr ?? throw new ArgumentNullException(nameof(hashExpr));
    }
}
