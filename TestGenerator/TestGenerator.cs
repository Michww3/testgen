namespace TestGenerator;

public static class TestGenerator
{
    public static string Generate(string className, string modelInterface, string modelClass, Param[] p)
    {
        var sb = new System.Text.StringBuilder();

        int n = p.Length;
        int total = 1 << n;

        const string I1 = "    ";
        const string I2 = "        ";

        // --- 1. From model ---
        sb.AppendLine("[Fact]");
        sb.AppendLine("public void ProduceCorrectHashFromModel()");
        sb.AppendLine("{");

        foreach (var param in p)
            sb.AppendLine($"{I1}{param.Type} {param.Name} = {param.Init};");

        sb.AppendLine();

        sb.AppendLine($"{I1}{modelInterface} model = new {modelClass}(");
        for (int i = 0; i < n; i++)
        {
            string comma = i < n - 1 ? "," : "";
            sb.AppendLine($"{I2}{p[i].Name}{comma}");
        }
        sb.AppendLine($"{I1});");
        sb.AppendLine();

        sb.AppendLine($"{I1}{className} expected = new {className}(model);");
        sb.AppendLine($"{I1}{className} actual = new {className}(model);");
        sb.AppendLine();
        sb.AppendLine($"{I1}Assert.True(expected.SequenceEqual(actual));");
        sb.AppendLine("}");
        sb.AppendLine();

        // --- 2. All combinations (SORTED) ---

        int[] masks = new int[total];
        for (int i = 0; i < total; i++)
            masks[i] = i;

        Array.Sort(masks, (a, b) =>
        {
            int cmp = CountBits(a).CompareTo(CountBits(b));
            return cmp != 0 ? cmp : a.CompareTo(b);
        });

        foreach (var mask in masks)
        {
            sb.AppendLine("[Fact]");
            sb.AppendLine($"public void ProduceCorrectHashFrom{MaskName(mask, p)}()");
            sb.AppendLine("{");

            foreach (var param in p)
                sb.AppendLine($"{I1}{param.Type} {param.Name} = {param.Init};");

            sb.AppendLine();

            sb.AppendLine($"{I1}{modelInterface} model = new {modelClass}(");
            for (int i = 0; i < n; i++)
            {
                string comma = i < n - 1 ? "," : "";
                sb.AppendLine($"{I2}{p[i].Name}{comma}");
            }
            sb.AppendLine($"{I1});");

            sb.AppendLine();

            sb.AppendLine($"{I1}{className} expected = new {className}(model);");

            sb.AppendLine($"{I1}{className} actual = new {className}(");
            for (int i = 0; i < n; i++)
            {
                bool isHash = (mask & (1 << i)) != 0;
                string value = isHash ? p[i].HashExpr : p[i].Name;

                string comma = i < n - 1 ? "," : "";
                sb.AppendLine($"{I2}{value}{comma}");
            }
            sb.AppendLine($"{I1});");

            sb.AppendLine();
            sb.AppendLine($"{I1}Assert.True(expected.SequenceEqual(actual));");
            sb.AppendLine("}");
            sb.AppendLine();
        }
        return sb.ToString();
    }
    static int CountBits(int x)
    {
        int count = 0;
        while (x != 0)
        {
            count += x & 1;
            x >>= 1;
        }
        return count;
    }

    static string MaskName(int mask, Param[] p)
    {
        if (mask == (1 << p.Length) - 1)
            return "Hashes";

        string name = "";

        for (int i = 0; i < p.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
                name += UpperFirst(p[i].Name) + "Hash";
        }

        return name == "" ? "Values" : name;
    }

    static string UpperFirst(string s)
    {
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}