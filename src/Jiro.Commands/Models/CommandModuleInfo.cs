namespace Jiro.Commands.Models;

/// <summary>
/// Contains information about a command module and its commands.
/// </summary>
public class CommandModuleInfo
{
	/// <summary>
	/// Gets the name of the command module.
	/// </summary>
	public string Name { get; private set; } = string.Empty;

	/// <summary>
	/// Gets the dictionary of commands in the module.
	/// </summary>
	public Dictionary<string, CommandInfo> Commands { get; private set; } = new();

	/// <summary>
	/// Sets the name of the command module.
	/// </summary>
	/// <param name="name">The name to set.</param>
	public void SetName(string name)
	{
		Name = name;
	}

	/// <summary>
	/// Adds the specified commands to the module.
	/// </summary>
	/// <param name="commands">The list of commands to add.</param>
	public void SetCommands(List<CommandInfo> commands)
	{
		foreach (var command in commands)
		{
			Commands.TryAdd(command.Name, command);
		}
	}
}
