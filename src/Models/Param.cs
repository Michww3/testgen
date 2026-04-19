using System;

namespace Testgen.Models;

public sealed record Param
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Init { get; set; } = string.Empty;
    public string HashExpr { get; set; } = string.Empty;

    public Param() { }

    public Param(string type, string name, string init, string hashExpr)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Init = init ?? throw new ArgumentNullException(nameof(init));
        HashExpr = hashExpr ?? throw new ArgumentNullException(nameof(hashExpr));
    }
}
