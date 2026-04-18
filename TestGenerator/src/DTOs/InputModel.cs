using System.Collections.Generic;

namespace TestGenerator.src.DTOs
{
    public class InputModel
    {
        public string? ClassName { get; set; }
        public string? ModelClass { get; set; }
        public string? ModelInterface { get; set; }
        public string? ConfigPath { get; set; }
        public string? OutputPath { get; set; }
        public List<TestParam> Parameters { get; set; } = new();
    }
}