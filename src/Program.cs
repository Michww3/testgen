using System;
using System.IO;
using System.Text.Json;
using testgen.DTOs;

namespace testgen;

public static class Program
{
    public static void Main(string[] args)
    {
        if (HandleInit(args))
        {
            return;
        }

        InputModel input = ParseArgs(args);

        input = LoadFromJson(input);

        Validate(input);

        Normalize(input);

        string result = Generate(input);

        WriteOutput(result, input);
    }

    private static bool HandleInit(string[] args)
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


    private static InputModel ParseArgs(string[] args)
    {
        InputModel input = new InputModel();

        foreach (string arg in args)
        {
            if (arg.StartsWith("--config=", StringComparison.Ordinal))
            {
                input.ConfigPath = arg.Split('=')[1];
            }
            else if (arg.StartsWith("--class=", StringComparison.Ordinal))
            {
                input.ModelClass = arg.Split('=')[1];
            }
            else if (arg.StartsWith("--hash-class=", StringComparison.Ordinal))
            {
                input.ClassName = arg.Split('=')[1];
            }
            else if (arg.StartsWith("--interface=", StringComparison.Ordinal))
            {
                input.ModelInterface = arg.Split('=')[1];
            }
            else if (arg.StartsWith("--out=", StringComparison.Ordinal))
            {
                input.OutputPath = arg.Split('=')[1];
            }
            else if (arg.StartsWith("--param=", StringComparison.Ordinal))
            {
                input.Parameters.Add(ParseParam(arg));
            }
        }

        return input;
    }

    private static TestParam ParseParam(string arg)
    {
        string[] parts = arg.Split('=')[1].Split(':');

        return parts.Length != 4
            ? throw new ArgumentException($"Invalid param: {arg}")
            : new TestParam(
            parts[1],
            parts[0],
            parts[2],
            parts[3]
        );
    }

    private static InputModel LoadFromJson(InputModel input)
    {
        if (input.ConfigPath == null)
        {
            return input;
        }

        if (!File.Exists(input.ConfigPath))
        {
            throw new FileNotFoundException($"Config not found: {input.ConfigPath}");
        }

        string json = File.ReadAllText(input.ConfigPath);

        GeneratorConfig config = JsonSerializer.Deserialize<GeneratorConfig>(json)
                     ?? throw new InvalidCastException("Invalid config");

        return new InputModel
        {
            ClassName = config.ClassName,
            ModelClass = config.ModelClass,
            ModelInterface = config.ModelInterface,
            Parameters = config.Params ?? [],
            OutputPath = input.OutputPath
        };
    }

    private static void Validate(InputModel input)
    {
        if (input.ModelClass == null)
        {
            throw new ArgumentException("--class or config.modelClass is required");
        }

        if (input.Parameters.Count == 0)
        {
            throw new ArgumentException("No params provided");
        }
    }

    private static void Normalize(InputModel input)
    {
        input.ClassName ??= input.ModelClass + "Hash";
        input.ModelInterface ??= "I" + input.ModelClass;
        input.OutputPath ??= input.ClassName + "Tests.cs";
    }

    private static string Generate(InputModel input)
    {
        return TestGenerator.Generate(
            input.ClassName!,
            input.ModelInterface!,
            input.ModelClass!,
            [.. input.Parameters]);
    }

    private static void WriteOutput(string result, InputModel input)
    {
        File.WriteAllText(input.OutputPath!, result);
        Console.WriteLine($"Tests generated: {input.OutputPath}");
    }

    private static void CreateTemplate(string path)
    {
        GeneratorConfig config = new GeneratorConfig
        {
            ModelClass = "MyModel",
            ModelInterface = "IMyModel",
            ClassName = "MyModelHash",
            Params =
            [
                new("IGuid","id","new Guid()","new Hash(id)"),
                new("IString","name","new String()","new Hash(name)")
            ]
        };

        string json = JsonSerializer.Serialize(
            config,
            JsonOptions);

        File.WriteAllText(path, json);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
}
