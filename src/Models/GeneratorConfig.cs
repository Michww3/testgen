using System;

namespace Testgen.Models;

public sealed record GeneratorConfig
{
    public string ModelName { get; init; }
    public string? ModelHash { get; init; }
    public string? ModelInterface { get; init; }
    public Param[] Params { get; init; }

    public GeneratorConfig(string modelName, string? modelHash, string? modelInterface, Param[] @params)
    {
        ModelName = modelName ?? throw new ArgumentNullException(nameof(modelName), "ModelName must be provided");
        ModelHash = modelHash;
        ModelInterface = modelInterface;
        Params = @params ?? throw new ArgumentNullException(nameof(@params), "Params must be provided");
    }
}
