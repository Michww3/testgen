using System.Collections.Generic;

namespace testgen.DTOs;

public class GeneratorConfig
{
    public string? ClassName { get; set; }
    public string? ModelClass { get; set; }
    public string? ModelInterface { get; set; }
    public List<Param>? Params { get; set; }
}
