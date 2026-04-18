using System.Collections.Generic;

namespace TestGenerator.src.DTOs;

public class GeneratorConfig
{
    public string? ClassName { get; set; }
    public string? ModelClass { get; set; }
    public string? ModelInterface { get; set; }
    public List<TestParam>? Params { get; set; }
}