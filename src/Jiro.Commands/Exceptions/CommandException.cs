namespace Jiro.Commands.Exceptions;

public class CommandException : Exception
{
    public string CommandName { get; set; }
    public CommandException(string commandName, string exceptionMessage) : base(exceptionMessage)
    {
        CommandName = commandName;
    }
}
