namespace Jiro.Commands.Attributes;

/// <summary>
/// Applied to a method within CommandModule class creates a command
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
/// <summary>
/// Attribute applied to a method within a CommandModule class to create a command.
/// </summary>
public class CommandAttribute : Attribute
{
	/// <summary>
	/// Gets the name of the command.
	/// </summary>
	public string CommandName { get; }

	/// <summary>
	/// Gets the type of the command.
	/// </summary>
	public CommandType CommandType { get; }

	/// <summary>
	/// Gets the syntax string for the command, if any.
	/// </summary>
	public string? CommandSyntax { get; }

	/// <summary>
	/// Gets the description of the command, if any.
	/// </summary>
	public string? CommandDescription { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="CommandAttribute"/> class.
	/// </summary>
	/// <param name="commandName">The name of the command.</param>
	/// <param name="commandType">The type of the command.</param>
	/// <param name="commandSyntax">The syntax string for the command.</param>
	/// <param name="commandDescription">The description of the command.</param>
	public CommandAttribute(string commandName, CommandType commandType = CommandType.Text, string? commandSyntax = "", string commandDescription = "")
	{
		CommandName = commandName;
		CommandType = commandType;
		CommandSyntax = commandSyntax;
		CommandDescription = commandDescription;
	}
}
