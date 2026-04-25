using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Testgen.Models;

namespace Testgen;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            if (HandleInit(args))
            {
                return 0;
            }

            InputModel input = ParseArgs(args);

            input = LoadFromJson(input);

            Validate(input);

            Normalize(input);

            string result = Generate(input);

            WriteOutput(result, input);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.Error.WriteLine();
            PrintUsage();
            return 1;
        }
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

        if (args.Any(a => a is "--help" or "-h") || args.Length == 0)
        {
            PrintUsage();
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
            else
            {
                throw new ArgumentException($"Unknown argument: {arg}");
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
                name: parts[0],
                type: parts[1],
                init: parts[2],
                hashExpr: parts[3]
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
            Params = [.. config.Params],
            OutputPath = input.OutputPath
        };
    }

    private static void Validate(InputModel input)
    {
        if (input.ModelName == null)
        {
            throw new ArgumentException("--name or config with ModelName is required");
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
            new GeneratorConfig(
                input.ModelName!,
                input.ModelHash,
                input.ModelInterface,
                [.. input.Params]
            )
        );
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
            modelName: "MyModel",
            modelHash: "MyModelHash",
            modelInterface: "IMyModel",
            [
                new(name: "id", type: "IGuid", init: "new Guid()", hashExpr: "new Hash(id)"),
                new(name: "name", type: "IString", init: "new String()", hashExpr: "new Hash(name)")
            ]
        );

        string json = JsonSerializer.Serialize(
            config,
            JsonOptions);

        File.WriteAllText(path, json);
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: testgen [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  init [path]           Create a config template (default: testgen.json)");
        Console.WriteLine("  --config=PATH         Path to JSON config");
        Console.WriteLine("  --name=NAME           Model class name");
        Console.WriteLine("  --hash=HASH           Hash class name (optional)");
        Console.WriteLine("  --interface=IFACE     Model interface (optional)");
        Console.WriteLine("  --param=PARAM         Parameter definition (name:type:init:hashExpr). Wrap in quotes if needed");
        Console.WriteLine("  --out=FILE            Output file (optional)");
        Console.WriteLine("  --help, -h            Show this help and exit");
    }


    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
}
