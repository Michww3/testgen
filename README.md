# testgen

CLI tool for generating hash-based unit tests for C# models.

`testgen` eliminates repetitive boilerplate when testing hash-based constructors by generating deterministic and exhaustive test cases for all parameter combinations.

---

## ✨ Features

* Generates `[Fact]` tests for all constructor combinations
* Supports **ladder-style test progression** (one hash at a time)
* Works via CLI or JSON configuration
* Outputs ready-to-compile `.cs` test files
* Designed for hash-driven domain models

---

## 📦 Installation

### As a global .NET tool

```bash
dotnet tool install --global testgen
```

### Local (from source)

```bash
dotnet pack -c Release
dotnet tool install --global --add-source ./nupkg testgen
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
testgen \
  --class=MyModel \
  --interface=IMyModel \
  --hash-class=MyModelHash \
  --param="id:IGuid:new Guid():new DeterminedHash(id)" \
  --param="chartId:IGuid:new Guid():new DeterminedHash(chartId)" \
  --out=MyModelHashTests.cs
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
  "modelClass": "MyModel",
  "modelInterface": "IMyModel",
  "className": "MyModelHash",
  "params": [
    {
      "type": "IGuid",
      "name": "id",
      "init": "new Guid()",
      "hashExpr": "new DeterminedHash(id)"
    },
    {
      "type": "IGuid",
      "name": "chartId",
      "init": "new Guid()",
      "hashExpr": "new DeterminedHash(chartId)"
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
| `--class`      | Model class name                                                           |
| `--interface`  | Model interface (optional)                                                 |
| `--hash-class` | Hash class name (optional)                                                 |
| `--param`      | Parameter definition (`name:type:init:hashExpr`, wrap in quotes if needed) |
| `--config`     | Path to JSON config                                                        |
| `--out`        | Output file                                                                |

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
