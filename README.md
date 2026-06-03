# ⚡ RuneEngine

<div align="center">

**A powerful and flexible workflow orchestration engine for .NET**

[![.NET Version](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

*Craft complex workflows with the elegance of runic symbols* ᚱᚢᚾᛖ

</div>

---

## 🌟 Overview

RuneEngine is a workflow engine designed for .NET 10, enabling developers to build, execute, and monitor complex business processes with ease.

## ✨ Key Features

- 🔄 **Dynamic Workflow Definition** - Define workflows programmatically or through declarative configuration
- ⚙ **Extensible Plugin System** - Create custom plugins for specific tasks and integrate them seamlessly
- 🧩 **Conditional Logic** - Support for branches and conditional execution

## 🚀 Quick Start

### Sample workflow
```json
{
  "name": "sample-wf",
  "version": "1.0",
  "runes": [
    {
      "id": "1",
      "name": "core.input_data",
      "inputs": {}
    },
    {
      "id": "2",
      "name": "core.get_property",
      "inputs": {
        "object": [ "1", "data" ],
        "propertyName": "Num1"
      }
    },
    {
      "id": "3",
      "name": "core.primitives.int",
      "inputs": {
        "value": 2
      }
    },
    {
      "id": "4",
      "name": "core.math.mul",
      "inputs": {
        "a": [ "2", "propertyValue" ],
        "b": [ "3", "value" ]
      }
    }
  ]
}
```

### Runes definition
`id` - Rune indentifier, must be unique within the workflow 

`name` - The name of the rune to execute (usually includes plugin prefix)

`inputs` - An object of key value pairs.
Key corresponds to the input name defined in rune. 
May be either a JSON-representation of static value or a connection to another rune output in array format,
e.g `[ "2", "propertyValue" ]` meaning "Take output of rune with Id 2 from ouput port named 'propertyValue'"

`metadata` - a free-form object of key value pairs that you can use in your custom runes for any purpose.

## 📦 Packages

- **RuneEngine** - Core workflow engine
- **RuneEngine.Plugins.Core** - Core plugins library

## 🤝 Contributing

Contributions are welcome!

⭐ If you find RuneEngine useful, please consider giving it a star!

<div align="center">
Built with ❤️ for the .NET community
</div>