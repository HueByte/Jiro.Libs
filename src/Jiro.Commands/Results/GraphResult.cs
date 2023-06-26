namespace Jiro.Commands.Results;

public sealed class GraphResult : ICommandResult
{
    public string? Message { get; set; }
    public object? Data { get; set; }
    public Dictionary<string, string>? Units { get; set; }
    public string? Note { get; set; } = string.Empty;
    public string? XAxis { get; set; } = string.Empty;
    public string? YAxis { get; set; } = string.Empty;

    private GraphResult(string message, object? data, Dictionary<string, string> units, string? xAxis = null, string? yAxis = null, string? note = null)
    {
        Message = message;
        Data = data;
        Units = units;
        XAxis = xAxis;
        YAxis = yAxis;
        Note = note;
    }

    public static GraphResult Create(string message, object? data, Dictionary<string, string> units, string? xAxis = null, string? yAxis = null, string? note = null)
    {
        return new GraphResult(message, data, units, xAxis, yAxis, note);
    }
}