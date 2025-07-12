using System.Text.Json.Serialization;

using Jiro.Commands.Results;

namespace Jiro.Commands;

[JsonDerivedType(typeof(GraphResult))]
[JsonDerivedType(typeof(TextResult))]
[JsonDerivedType(typeof(JsonResult))]
/// <summary>
/// Represents the result of a command execution.
/// </summary>
public interface ICommandResult
{
	/// <summary>
	/// Gets or sets the message associated with the command result.
	/// </summary>
	string? Message { get; set; }
}
