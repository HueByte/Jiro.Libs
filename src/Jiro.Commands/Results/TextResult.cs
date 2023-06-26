namespace Jiro.Commands.Results;

public sealed class TextResult : ICommandResult
{
    public string? Message { get; set; }

    private TextResult(string? message)
    {
        Message = message;
    }

    public static TextResult Create(string? message)
    {
        return new TextResult(message);
    }
}
