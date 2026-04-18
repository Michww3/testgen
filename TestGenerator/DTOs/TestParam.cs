namespace TestGenerator.DTOs;

public sealed record TestParam
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Init { get; set; } = string.Empty;
    public string HashExpr { get; set; } = string.Empty;

    public TestParam() { }

    public TestParam(string type, string name, string init, string hashExpr)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Init = init ?? throw new ArgumentNullException(nameof(init));
        HashExpr = hashExpr ?? throw new ArgumentNullException(nameof(hashExpr));
    }
}
