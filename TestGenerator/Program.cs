using System.Text.Json;
using TestGenerator.DTOs;

namespace TestGenerator;

public static class Program
{
    public static void Main(string[] args)
    {
        if (HandleInit(args))
            return;

        var input = ParseArgs(args);

        input = LoadFromJson(input);

        Validate(input);

        Normalize(input);

        string result = Generate(input);

        WriteOutput(result, input);
    }

    static bool HandleInit(string[] args)
    {
        if (args.Length > 0 && args[0] == "init")
        {
            string path = args.Length > 1 ? args[1] : "testgen.json";
            CreateTemplate(path);
            Console.WriteLine($"Config template created: {path}");
            return true;
        }

        return false;
    }


    static InputModel ParseArgs(string[] args)
    {
        var input = new InputModel();

        foreach (var arg in args)
        {
            if (arg.StartsWith("--config="))
                input.ConfigPath = arg.Split('=')[1];

            else if (arg.StartsWith("--class="))
                input.ModelClass = arg.Split('=')[1];

            else if (arg.StartsWith("--hash-class="))
                input.ClassName = arg.Split('=')[1];

            else if (arg.StartsWith("--interface="))
                input.ModelInterface = arg.Split('=')[1];

            else if (arg.StartsWith("--out="))
                input.OutputPath = arg.Split('=')[1];

            else if (arg.StartsWith("--param="))
                input.Parameters.Add(ParseParam(arg));
        }

        return input;
    }

    static TestParam ParseParam(string arg)
    {
        var parts = arg.Split('=')[1].Split(':');

        if (parts.Length != 4)
            throw new Exception($"Invalid param: {arg}");

        return new TestParam(
            parts[1],
            parts[0],
            parts[2],
            parts[3]
        );
    }

    static InputModel LoadFromJson(InputModel input)
    {
        if (input.ConfigPath == null)
            return input;

        if (!File.Exists(input.ConfigPath))
            throw new Exception($"Config not found: {input.ConfigPath}");

        var json = File.ReadAllText(input.ConfigPath);

        var config = JsonSerializer.Deserialize<GeneratorConfig>(json)
                     ?? throw new Exception("Invalid config");

        return new InputModel
        {
            ClassName = config.ClassName,
            ModelClass = config.ModelClass,
            ModelInterface = config.ModelInterface,
            Parameters = config.Params ?? new List<TestParam>(),
            OutputPath = input.OutputPath
        };
    }

    static void Validate(InputModel input)
    {
        if (input.ModelClass == null)
            throw new Exception("--class or config.modelClass is required");

        if (input.Parameters.Count == 0)
            throw new Exception("No params provided");
    }

    static void Normalize(InputModel input)
    {
        input.ClassName ??= input.ModelClass + "Hash";
        input.ModelInterface ??= "I" + input.ModelClass;
        input.OutputPath ??= input.ClassName + "Tests.cs";
    }

    static string Generate(InputModel input)
    {
        return TestGenerator.Generate(
            input.ClassName!,
            input.ModelInterface!,
            input.ModelClass!,
            input.Parameters.ToArray());
    }

    static void WriteOutput(string result, InputModel input)
    {
        File.WriteAllText(input.OutputPath!, result);
        Console.WriteLine($"Tests generated: {input.OutputPath}");
    }

    static void CreateTemplate(string path)
    {
        var config = new GeneratorConfig
        {
            ModelClass = "MyModel",
            ModelInterface = "IMyModel",
            ClassName = "MyModelHash",
            Params = new List<TestParam>
        {
                new("IGuid","id","new Guid()","new Hash(id)"),
                new("IString","name","new String()","new Hash(name)")
            }
        };

        JsonSerializerOptions options = new() { WriteIndented = true };
        var json = JsonSerializer.Serialize(
            config,
            options);

        File.WriteAllText(path, json);
    }
}
