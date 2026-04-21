using System;
using System.Collections.Generic;

namespace Testgen.Models;

public sealed record GeneratorConfig
{
    public string ModelName { get; init; }
    public string? ModelHash { get; init; }
    public string? ModelInterface { get; init; }
    public List<Param> Params { get; init; }

    public GeneratorConfig(string modelName, string? modelHash, string? modelInterface, List<Param> @params)
    {
        ModelName = modelName ?? throw new ArgumentNullException(nameof(modelName));
        ModelHash = modelHash;
        ModelInterface = modelInterface;
        Params = @params ?? throw new ArgumentNullException(nameof(@params));
    }
}
