namespace Jiro.Commands
{
	/// <summary>
	/// Specifies the type of command result.
	/// </summary>
	public enum CommandType
	{
		/// <summary>
		/// Represents a text command.
		/// </summary>
		Text,

		/// <summary>
		/// Represents a JSON command.
		/// </summary>
		Json,

		/// <summary>
		/// Represents a graph command.
		/// </summary>
		Graph,
		/// <summary>
		/// Represents an image command.
		/// </summary>
		Image
	}
}
