namespace Jiro.Commands.Results;

/// <summary>
/// Represents a text result for a command.
/// </summary>
public sealed class TextResult : ICommandResult
{
	/// <summary>
	/// Gets or sets the message of the result.
	/// </summary>
	public string? Message { get; set; }

	private TextResult(string? message)
	{
		Message = message;
	}

	/// <summary>
	/// Creates a new <see cref="TextResult"/> instance with the specified message.
	/// </summary>
	/// <param name="message">The message for the result.</param>
	/// <returns>A new <see cref="TextResult"/> instance.</returns>
	public static TextResult Create(string? message)
	{
		return new TextResult(message);
	}
}
