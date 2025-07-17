# Jiro.Commands

> **A flexible command and plugin framework for [Jiro](https://github.com/HueByte/Jiro)**

Jiro.Commands provides a robust, extensible system for building plugins and runtime commands for the Jiro platform. It enables modular development, dynamic feature loading, and a clean separation of concerns for your application.

---

## Features

- **Plugin Management**: Standardized loading/unloading of plugins at runtime.
- **Runtime Command Creation**: Add new commands without rebuilding or redeploying your app.
- **Dependency Injection**: Full support for DI in commands and plugins.
- **Custom Controllers**: Easily extend your API surface with plugin controllers.
- **Strong Typing & Parsing**: Type-safe command parameters and extensible parsing.
- **XML Documentation**: All public APIs are fully documented for IntelliSense and doc generation.

---

## Getting Started

### Installation

Install via NuGet Package Manager:

```bash
dotnet add package Jiro.Commands
```

Or via Package Manager Console:

```powershell
Install-Package Jiro.Commands
```

---

## Plugin Development

Create a plugin by implementing the `IPlugin` interface:

```csharp
public class PluginMain : IPlugin
{
    public string PluginName { get; } = "PluginMain";

    // Optional: Register configuration files
    public void RegisterAppConfigs(ConfigurationManager builder)
        => builder.AddJsonFile("example.config.json", optional: true, reloadOnChange: true);

    // Optional: Register middleware
    public void RegisterAppExtensions(IApplicationBuilder app)
        => app.UsePluginMiddleware();

    // Required: Register services
    public void RegisterServices(IServiceCollection services)
        => services.AddScoped<IPluginService, PluginService>();
}
```

**Plugin requirements:**

- Implement `IPlugin`.
- Provide a unique `PluginName`.
- Register any services, configs, or middleware as needed.

---

## Custom Controllers

Extend the API by inheriting from `BaseController`:

```csharp
public class PluginController : BaseController
{
    [HttpGet("PluginTest")]
    public IActionResult PluginTest() => Ok("Plugin Controller Executed");
}
```

---

## Custom Commands

Define commands by using attributes and implementing `ICommandBase`:

```csharp
[CommandModule("PluginCommand")]
public class PluginCommand : ICommandBase
{
    private readonly IPluginService _pluginService;
    public PluginCommand(IPluginService pluginService) => _pluginService = pluginService;

    [Command("PluginTest", commandSyntax: "PluginTest", commandDescription: "Tests plugin command")]
    public async Task<ICommandResult> PluginTest()
    {
        _pluginService.ServiceTest();
        await Task.Delay(1000);
        return TextResult.Create("Plugin Command Executed");
    }
}
```

**Command requirements:**

- Class must have `[CommandModule]` attribute.
- Class must implement `ICommandBase`.
- Command methods must have `[Command]` attribute (with at least a name).
- Command methods must return `Task<ICommandResult>`.
- Use `TextResult.Create`, `ImageResult.Create`, `JsonResult.Create`, or `GraphResult.Create` for results.

---

## API Reference

- All public APIs are documented with XML comments.
- See the source code for detailed usage and extension points.

---

## Contributing

Contributions, issues, and feature requests are welcome! Please open an issue or pull request on [GitHub](https://github.com/HueByte/Jiro.Libs).

### Development Setup

This project uses [EditorConfig](https://editorconfig.org/) to maintain consistent coding styles. Make sure your editor supports EditorConfig or install the appropriate extension:

- **VS Code**: Install the [EditorConfig extension](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)
- **Visual Studio**: Built-in support (2017+)
- **Other editors**: See [EditorConfig.org](https://editorconfig.org/) for editor-specific setup

### Code Formatting

- Run `dotnet format` before committing to ensure code follows the project's style guidelines
- The CI pipeline will verify that code formatting is consistent

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
