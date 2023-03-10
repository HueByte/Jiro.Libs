namespace Jiro.Commands.Models;

/// <summary>
/// Module information containing its commands
/// </summary>
public class CommandModuleInfo
{
    public string Name { get; private set; } = string.Empty;
    public Dictionary<string, CommandInfo> Commands { get; private set; } = new();

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetCommands(List<CommandInfo> commands)
    {
        foreach (var command in commands)
        {
            Commands.TryAdd(command.Name, command);
        }
    }
}
