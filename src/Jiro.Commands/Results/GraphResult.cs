namespace Jiro.Commands.Results;

/// <summary>
/// Represents a graph result for a command.
/// </summary>
public sealed class GraphResult : ICommandResult
{
    /// <summary>
    /// Gets or sets the message of the result.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the data for the graph.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the units for the graph data.
    /// </summary>
    public Dictionary<string, string>? Units { get; set; }

    /// <summary>
    /// Gets or sets an optional note for the result.
    /// </summary>
    public string? Note { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the label for the X axis.
    /// </summary>
    public string? XAxis { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the label for the Y axis.
    /// </summary>
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

    /// <summary>
    /// Creates a new <see cref="GraphResult"/> instance with the specified parameters.
    /// </summary>
    /// <param name="message">The message for the result.</param>
    /// <param name="data">The data for the graph.</param>
    /// <param name="units">The units for the graph data.</param>
    /// <param name="xAxis">The label for the X axis.</param>
    /// <param name="yAxis">The label for the Y axis.</param>
    /// <param name="note">An optional note for the result.</param>
    /// <returns>A new <see cref="GraphResult"/> instance.</returns>
    public static GraphResult Create(string message, object? data, Dictionary<string, string> units, string? xAxis = null, string? yAxis = null, string? note = null)
    {
        return new GraphResult(message, data, units, xAxis, yAxis, note);
    }
}