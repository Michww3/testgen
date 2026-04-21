using System;
using System.IO;
using System.Text.Json;
using Testgen.Models;

namespace Testgen;

public static class Program
{
    public static void Main(string[] args)
    {
        //args = ["init"];
        args = ["--config=testgen.json"];
        //args = ["--name=MyModelCli", "--interface=IMyModelCli", "--param=id:IGuid:new Guid():new Hash(id)", "--param=name:IString:new String():new Hash(name)", "--out=output.json"];
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
            else if (arg.StartsWith("--name=", StringComparison.Ordinal))
            {
                input.ModelName = arg.Split('=')[1];
            }
            else if (arg.StartsWith("--hash=", StringComparison.Ordinal))
            {
                input.ModelHash = arg.Split('=')[1];
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
                input.Params.Add(ParseParam(arg));
            }
        }

        return input;
    }

    private static Param ParseParam(string arg)
    {
        string[] parts = arg.Split('=')[1].Split(':');

        return parts.Length != 4
            ? throw new ArgumentException($"Invalid param: {arg}")
            : new Param(
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
            ModelName = config.ModelName,
            ModelHash = config.ModelHash,
            ModelInterface = config.ModelInterface,
            Params = config.Params ?? [],
            OutputPath = input.OutputPath
        };
    }

    private static void Validate(InputModel input)
    {
        if (input.ModelName == null)
        {
            throw new ArgumentException("--class or config with ModelName is required");
        }

        if (input.Params.Count == 0)
        {
            throw new ArgumentException("No params provided");
        }
    }

    private static void Normalize(InputModel input)
    {
        input.ModelHash ??= input.ModelName + "Hash";
        input.ModelInterface ??= "I" + input.ModelName;
        input.OutputPath ??= input.ModelName + "Tests.cs";
    }

    private static string Generate(InputModel input)
    {
        return TestGenerator.Generate(
            input.ModelName!,
            input.ModelInterface!,
            input.ModelHash!,
            [.. input.Params]);
    }

    private static void WriteOutput(string result, InputModel input)
    {
        File.WriteAllText(input.OutputPath!, result);
        Console.WriteLine($"Tests generated: {input.OutputPath}");
    }

    private static void CreateTemplate(string path)
    {
        GeneratorConfig config = new GeneratorConfig
        (
            "MyModel",
            "MyModelHash",
            "IMyModel",
            [
                new("IGuid","id","new Guid()","new Hash(id)"),
                new("IString","name","new String()","new Hash(name)")
            ]
        );

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
