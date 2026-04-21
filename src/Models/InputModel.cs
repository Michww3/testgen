using System.Collections.Generic;

namespace Testgen.Models;

public class InputModel
{
    public string? ModelName { get; set; }
    public string? ModelHash { get; set; }
    public string? ModelInterface { get; set; }
    public string? ConfigPath { get; set; }
    public string? OutputPath { get; set; }
    public List<Param> Params { get; set; } = [];
}
