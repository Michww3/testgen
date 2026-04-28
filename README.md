# testgen

> CLI tool for generating hash-based unit tests for C# models.

`testgen` eliminates repetitive boilerplate when testing hash-based constructors by generating deterministic and exhaustive test cases for all parameter combinations.

---

## 📚 Table of Contents

- [✨ Features](#-features)
- [📦 Installation](#-installation)
- [🚀 Quick Start](#-quick-start)
- [🖥 CLI Usage](#-cli-usage)
- [⚙️ Configuration](#️-configuration)
- [🧬 Generated Output](#-generated-output)
- [🧠 How It Works](#-how-it-works)
- [📊 Before / After](#-before--after)
- [📋 Arguments](#-arguments)
- [📁 Examples](#-examples)
- [📄 License](#-license)
- [🤝 Contributing](#-contributing)


---

## ✨ Features

* Generates `[Fact]` tests for all constructor combinations
* Supports **ladder-style test progression** (one hash at a time)
* Works via CLI or JSON configuration
* Outputs `.cs` test files
* Designed for hash-driven domain models

---

## 📦 Installation

### As a global .NET tool

```bash
dotnet tool install --global testgen
```

### Local (from src folder)

```bash
dotnet pack -c Release
dotnet tool install --global --add-source bin/Release testgen
```

---

## 🚀 Quick Start

### 1. Generate config

```bash
testgen init
```

Creates:

```text
testgen.json
```

---

### 2. Generate tests

```bash
testgen --config=testgen.json
```

Output:

```text
MyModelHashTests.cs
```

---

## 🖥 CLI Usage

```bash
testgen --name=MyModel --interface=IMyModel --hash=MyModelHash --param="id:IGuid:new Guid():new DeterminedHash(id)" --param="chartId:IGuid:new Guid():new DeterminedHash(chartId)" --out=MyModelHashTests.cs
```

---

## ⚠️ Passing Parameters via CLI

Each parameter must follow this format:

```text
name:type:init:hashExpr
```

Example:

```bash
--param="id:IGuid:new Guid():new DeterminedHash(id)"
```

### ❗ Important: spaces in expressions

Shell splits arguments by spaces. If your expressions contain spaces (e.g. `new Guid()`), you **must wrap the entire parameter in quotes**.

### ✅ Correct

```bash
--param="id:IGuid:new Guid():new DeterminedHash(id)"
```

### ❌ Incorrect

```bash
--param=id:IGuid:new Guid():new DeterminedHash(id)
```

This will be parsed incorrectly as multiple arguments.

---

### 💡 Tip

For complex models, prefer JSON config:

```bash
testgen --config=testgen.json
```

It is more readable and less error-prone.

---

## ⚙️ Configuration

Example `testgen.json`:

```json
{
  "ModelName": "MyModel",
  "ModelHash": "MyModelHash",
  "ModelInterface": "IMyModel",
  "Params": [
    {
      "Type": "IGuid",
      "Name": "id",
      "Init": "new Guid()",
      "HashExpr": "new Hash(id)"
    },
    {
      "Type": "IString",
      "Name": "name",
      "Init": "new String()",
      "HashExpr": "new Hash(name)"
    }
  ]
}
```

---

## 🧪 Generated Output

### Value-based test

```csharp
[Fact]
public void ProduceCorrectHashFromValues()
{
    IGuid id = new Guid();
    IGuid chartId = new Guid();

    IMyModel model = new MyModel(
        id,
        chartId
    );

    MyModelHash expected = new MyModelHash(model);
    MyModelHash actual = new MyModelHash(
        id,
        chartId
    );

    Assert.True(expected.SequenceEqual(actual));
}
```

### Hash-based test

```csharp
[Fact]
public void ProduceCorrectHashFromIdHash()
{
    IGuid id = new Guid();
    IGuid chartId = new Guid();

    IMyModel model = new MyModel(
        id,
        chartId
    );

    MyModelHash expected = new MyModelHash(model);
    MyModelHash actual = new MyModelHash(
        new DeterminedHash(id),
        chartId
    );

    Assert.True(expected.SequenceEqual(actual));
}
```

---

## 🧠 How It Works

For a model with `N` parameters, `testgen`:

1. Builds a model using raw values
2. Generates expected hash via model constructor
3. Iterates through parameter combinations
4. Replaces values with hashes according to mask
5. Verifies equality using `SequenceEqual`

Tests are sorted by number of hash parameters for better readability.

---

## 📊 Before / After

### ❌ Manual approach

* Write dozens of repetitive tests
* Easy to miss combinations
* Hard to maintain

### ✅ With testgen

```bash
testgen --config=testgen.json
```

* All combinations generated automatically
* Deterministic and consistent
* Minimal maintenance

---

## 📋 Arguments

| Argument       | Description                                                                |
| -------------- | -------------------------------------------------------------------------- |
| `--name`       | Model class name                                                           |
| `--hash`       | Hash class name (optional)                                                 |
| `--interface`  | Model interface (optional)                                                 |
| `--param`      | Parameter definition (`name:type:init:hashExpr`, wrap in quotes if needed) |
| `--config`     | Path to JSON config                                                        |
| `--out`        | Output file (optional)                                                     |
| `--help`, `-h` | Show help                                                                  |

---

## 📁 Examples

See `/examples`:

```
examples/
├── testgen.json
└── generated-tests.cs
```

---

## 📄 License

MIT License

---

## 🤝 Contributing

Contributions are welcome.

If you find a bug or want a feature — open an issue or submit a PR.

---
