# Jiro.Commands
An command base and plugin manager for [Jiro](https://github.com/HueByte/Jiro).
`Jiro.Commands` allows you to easily create plugins for Jiro while giving you the flexibility to create custom commands at runtime.

With the plugin management feature, Jiro provides a standardized way of loading and unloading plugins. This allows you to easily add new features to your application without having to rebuild it from scratch. 

With the command creation functionality, you can easily create custom commands at runtime. This feature allows you to add new functionality to your application without having to rebuild or redeploy it. Simply create a new command and it will instantly be available for use.

## Plugin creation
To create a plugin, you have to create your own class that will inherit `IPlugin` interface

```cs
public class PluginMain : IPlugin
{
    public string PluginName { get; } = "PluginMain";

    // optional
    public void RegisterAppConfigs(ConfigurationManager builder)
    {
        builder.AddJsonFile("example.config.json", optional: true, reloadOnChange: true);
    }

    // optional
    public void RegisterAppExtensions(IApplicationBuilder app)
    {
        app.UsePluginMiddleware();
    }

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IPluginService, PluginService>();
    }
}
```

- `public string PluginName { get; } = "PluginMain";` - This property returns the name of the plugin.

`public void RegisterAppConfigs(ConfigurationManager builder)` - This method is optional and allows the plugin to register configuration files. In this case, an example example.config.json file is registered as a JSON file that can be reloaded on changes.

`public void RegisterAppExtensions(IApplicationBuilder app)` - This method is optional and allows the plugin to register additional middleware for the application. Here, the `UsePluginMiddleware` method is registered as an extension method for an `IApplicationBuilder` instance.

`public void RegisterServices(IServiceCollection services)` - This method is used to register services that the plugin provides or requires. In this case, an instance of `PluginService` class is registered as a service that can be injected using the `AddScoped` extension method for `IServiceCollection`.

### Custom Controllers
To create a custom controller, you have to create your own class that will inherit `BaseController` class

```cs
public class PluginController : BaseController
{
    public PluginController() { }

    [HttpGet("PluginTest")]
    public IActionResult PluginTest()
    {
        return Ok("Plugin Controller Executed");
    }
}
```

### Custom Commands
*Commands have scoped lifetime by default.*

```cs
[CommandModule("PluginCommand")]
public class PluginCommand : ICommandBase
{
    private readonly IPluginService _pluginService;
    public PluginCommand(IPluginService pluginService)
    {
        _pluginService = pluginService;
    }

    [Command("PluginTest", commandSyntax: "PluginTest", commandDescription: "Tests plugin command")]
    public async Task<ICommandResult> PluginTest()
    {
        _pluginService.ServiceTest();

        await Task.Delay(1000);
        return TextResult.Create("Plugin Command Executed");
    }
}
```

- `[CommandModule("PluginCommand")]` - This attribute is used to define a command module, which acts as a container for commands. In this case, the module name is "PluginCommand".

- `public class PluginCommand : ICommandBase` - This class implements the ICommandBase interface and is a command.

- `public PluginCommand(IPluginService pluginService)` - The command's constructor injects an instance of IPluginService as a dependency.

- `[Command("PluginTest", commandSyntax: "PluginTest", commandDescription: "Tests plugin command")]` - This attribute is used to define a command within the PluginCommand module. Here, the command name is "PluginTest" and it has a description "Tests plugin command". It also provides the command syntax as `"PluginTest"`.

- `return TextResult.Create("Plugin Command Executed");` - Finally, returns the result of the command execution. In this case, it creates a new instance of a TextResult object that contains the string message "Plugin Command Executed".

The requirements for commands:
- Class must contain `[CommandModuleAttribute]`
- Class must inherit `ICommandBase`
- Command Method must contain `[CommandAttribute]` with at least a command name
- Command Method must return `Task<ICommandResult>`
- Use `TextResult.Create`, `ImageResult.Create`, `GraphResult.Create` for creating `ICommandResult`
