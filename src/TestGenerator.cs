using System;
using System.Text;
using Testgen.Models;

namespace Testgen;

public static class TestGenerator
{
#pragma warning disable IDE0058
    public static string Generate(GeneratorConfig config)
    {
        StringBuilder sb = new StringBuilder();

        int n = config.Params.Length;
        int total = 1 << n;

        const string i1 = "    ";
        const string i2 = "        ";

        //Model
        sb.AppendLine("[Fact]");
        sb.AppendLine("public void ProduceCorrectHashFromModel()");
        sb.AppendLine("{");

        foreach (Param param in config.Params)
        {
            sb.AppendLine($"{i1}{param.Type} {param.Name} = {param.Init};");
        }

        sb.AppendLine();

        sb.AppendLine($"{i1}{config.ModelInterface} model = new {config.ModelHash}(");
        for (int i = 0; i < n; i++)
        {
            string comma = i < n - 1 ? "," : "";
            sb.AppendLine($"{i2}{config.Params[i].Name}{comma}");
        }
        sb.AppendLine($"{i1});");
        sb.AppendLine();

        sb.AppendLine($"{i1}{config.ModelName} expected = new {config.ModelName}(model);");
        sb.AppendLine($"{i1}{config.ModelName} actual = new {config.ModelName}(model);");
        sb.AppendLine();
        sb.AppendLine($"{i1}Assert.True(expected.SequenceEqual(actual));");
        sb.AppendLine("}");
        sb.AppendLine();

        //Ctor combinations
        int[] masks = new int[total];
        for (int i = 0; i < total; i++)
        {
            masks[i] = i;
        }

        Array.Sort(masks, (a, b) =>
        {
            int cmp = CountBits(a).CompareTo(CountBits(b));
            return cmp != 0 ? cmp : a.CompareTo(b);
        });

        foreach (int mask in masks)
        {
            sb.AppendLine("[Fact]");
            sb.AppendLine($"public void ProduceCorrectHashFrom{MaskName(mask, config.Params)}()");
            sb.AppendLine("{");

            foreach (Param param in config.Params)
            {
                sb.AppendLine($"{i1}{param.Type} {param.Name} = {param.Init};");
            }

            sb.AppendLine();

            sb.AppendLine($"{i1}{config.ModelInterface} model = new {config.ModelHash}(");
            for (int i = 0; i < n; i++)
            {
                string comma = i < n - 1 ? "," : "";
                sb.AppendLine($"{i2}{config.Params[i].Name}{comma}");
            }
            sb.AppendLine($"{i1});");

            sb.AppendLine();

            sb.AppendLine($"{i1}{config.ModelName} expected = new {config.ModelName}(model);");

            sb.AppendLine($"{i1}{config.ModelName} actual = new {config.ModelName}(");
            for (int i = 0; i < n; i++)
            {
                bool isHash = (mask & (1 << i)) != 0;
                string value = isHash ? config.Params[i].HashExpr : config.Params[i].Name;
                string comma = i < n - 1 ? "," : "";
                sb.AppendLine($"{i2}{value}{comma}");
            }
            sb.AppendLine($"{i1});");

            sb.AppendLine();
            sb.AppendLine($"{i1}Assert.True(expected.SequenceEqual(actual));");
            sb.AppendLine("}");
            sb.AppendLine();
        }
        return sb.ToString();
    }
#pragma warning restore IDE0058
    private static int CountBits(int x)
    {
        int count = 0;
        while (x != 0)
        {
            count += x & 1;
            x >>= 1;
        }
        return count;
    }

    private static string MaskName(int mask, Param[] p)
    {
        if (mask == (1 << p.Length) - 1)
        {
            return "Hashes";
        }

        string name = "";

        for (int i = 0; i < p.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                name += UpperFirst(p[i].Name) + "Hash";
            }
        }

        return name == "" ? "Values" : name;
    }

    private static string UpperFirst(string s)
    {
        return char.ToUpper(s[0], System.Globalization.CultureInfo.InvariantCulture) + s[1..];
    }
}
