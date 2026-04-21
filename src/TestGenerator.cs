using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testgen.Models;

namespace Testgen;

public static class TestGenerator
{
    public static string Generate(GeneratorConfig config)
    {
        StringBuilder sb = new StringBuilder();

        // 1. Test: from model
        _ = sb.AppendLine(BuildModelTest(config));
        _ = sb.AppendLine();

        // 2. Test: all parameters combinations (2n)
        int totalCombinations = 1 << config.Params.Length;
        IOrderedEnumerable<int> combinations = Enumerable.Range(0, totalCombinations)
            .OrderBy(CountBits)
            .ThenBy(mask => mask);

        foreach (int mask in combinations)
        {
            _ = sb.AppendLine(BuildParameterTest(config, mask));
            _ = sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string BuildModelTest(GeneratorConfig config)
    {
        const string i1 = "    ";
        const string i2 = "        ";

        string declarations = string.Join(Environment.NewLine,
            config.Params.Select(p => $"{i1}{p.Type} {p.Name} = {p.Init};"));

        string modelArgs = string.Join($",{Environment.NewLine}",
            config.Params.Select(p => $"{i2}{p.Name}"));

        return $"[Fact]{Environment.NewLine}" +
               $"public void ProduceCorrectHashFromModel(){Environment.NewLine}" +
               $"{{{Environment.NewLine}" +
               $"{declarations}{Environment.NewLine}{Environment.NewLine}" +
               $"{i1}{config.ModelInterface} model = new {config.ModelHash}({Environment.NewLine}" +
               $"{modelArgs}{Environment.NewLine}" +
               $"{i1});{Environment.NewLine}{Environment.NewLine}" +
               $"{i1}{config.ModelName} expected = new {config.ModelName}(model);{Environment.NewLine}" +
               $"{i1}{config.ModelName} actual = new {config.ModelName}(model);{Environment.NewLine}{Environment.NewLine}" +
               $"{i1}Assert.True(expected.SequenceEqual(actual));{Environment.NewLine}" +
               $"}}";
    }

    private static string BuildParameterTest(GeneratorConfig config, int mask)
    {
        const string i1 = "    ";
        const string i2 = "        ";

        string declarations = string.Join(Environment.NewLine,
            config.Params.Select(p => $"{i1}{p.Type} {p.Name} = {p.Init};"));

        string modelArgs = string.Join($",{Environment.NewLine}",
            config.Params.Select(p => $"{i2}{p.Name}"));

        // HashExpr or Name depending on mask
        string actualArgs = string.Join($",{Environment.NewLine}",
            config.Params.Select((p, i) => $"{i2}{((mask & (1 << i)) != 0 ? p.HashExpr : p.Name)}"));

        string suffix = GetTestNameSuffix(mask, config.Params);

        return $"[Fact]{Environment.NewLine}" +
               $"public void ProduceCorrectHashFrom{suffix}(){Environment.NewLine}" +
               $"{{{Environment.NewLine}" +
               $"{declarations}{Environment.NewLine}{Environment.NewLine}" +
               $"{i1}{config.ModelInterface} model = new {config.ModelHash}({Environment.NewLine}" +
               $"{modelArgs}{Environment.NewLine}" +
               $"{i1});{Environment.NewLine}{Environment.NewLine}" +
               $"{i1}{config.ModelName} expected = new {config.ModelName}(model);{Environment.NewLine}" +
               $"{i1}{config.ModelName} actual = new {config.ModelName}({Environment.NewLine}" +
               $"{actualArgs}{Environment.NewLine}" +
               $"{i1});{Environment.NewLine}{Environment.NewLine}" +
               $"{i1}Assert.True(expected.SequenceEqual(actual));{Environment.NewLine}" +
               $"}}";
    }

    private static string GetTestNameSuffix(int mask, Param[] parameters)
    {
        if (mask == (1 << parameters.Length) - 1)
        {
            return "Hashes";
        }

        IEnumerable<string> hashedNames = parameters
            .Where((_, i) => (mask & (1 << i)) != 0)
            .Select(p => char.ToUpper(p.Name[0], System.Globalization.CultureInfo.InvariantCulture) + p.Name[1..] + "Hash");

        // If mask == 0, return "Values"
        return string.Concat(hashedNames.DefaultIfEmpty("Values"));
    }

    private static int CountBits(int x)
    {
        int count = 0;
        while (x != 0)
        { count += x & 1; x >>= 1; }
        return count;
    }
}
