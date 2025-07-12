namespace Jiro.Commands.Models;

/// <summary>
/// Global container for all commands and modules.
/// </summary>
public class CommandsContext
{
    /// <summary>
    /// Gets the name of the default command.
    /// </summary>
    public string DefaultCommand { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the dictionary of command modules.
    /// </summary>
    public Dictionary<string, CommandModuleInfo> CommandModules { get; private set; } = new();

    /// <summary>
    /// Gets the dictionary of all commands.
    /// </summary>
    public Dictionary<string, CommandInfo> Commands { get; private set; } = new();

    /// <summary>
    /// Sets the default command name.
    /// </summary>
    /// <param name="defaultCommand">The default command name.</param>
    public void SetDefaultCommand(string defaultCommand) => DefaultCommand = defaultCommand;

    /// <summary>
    /// Adds the specified commands to the global context.
    /// </summary>
    /// <param name="commands">The list of commands to add.</param>
    public void AddCommands(List<CommandInfo> commands)
    {
        foreach (var command in commands)
        {
            Commands.TryAdd(command.Name, command);
        }
    }

    /// <summary>
    /// Adds the specified command modules to the global context.
    /// </summary>
    /// <param name="commandModules">The list of command modules to add.</param>
    public void AddModules(List<CommandModuleInfo> commandModules)
    {
        foreach (var commandModule in commandModules)
        {
            CommandModules.TryAdd(commandModule.Name, commandModule);
        }
    }
}
