namespace Jiro.Commands.Exceptions;

/// <summary>
/// Represents an exception that occurs during command execution.
/// </summary>
public class CommandException : Exception
{
    /// <summary>
    /// Gets or sets the name of the command that caused the exception.
    /// </summary>
    public string CommandName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandException"/> class.
    /// </summary>
    /// <param name="commandName">The name of the command that caused the exception.</param>
    /// <param name="exceptionMessage">The exception message.</param>
    public CommandException(string commandName, string exceptionMessage) : base(exceptionMessage)
    {
        CommandName = commandName;
    }
}
