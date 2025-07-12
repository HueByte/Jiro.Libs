namespace Jiro.Commands.Models
{
	/// <summary>
	/// Represents the response returned after executing a command.
	/// </summary>
	public class CommandResponse
	{
		/// <summary>
		/// Gets or sets the name of the executed command.
		/// </summary>
		public string? CommandName { get; set; }

		/// <summary>
		/// Gets or sets the type of the command.
		/// </summary>
		public CommandType CommandType { get; set; }

		/// <summary>
		/// Gets or sets the result of the command execution.
		/// </summary>
		public ICommandResult? Result { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the command executed successfully.
		/// </summary>
		public bool IsSuccess { get; set; }
	}
}
