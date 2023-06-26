using System.Text.Json.Serialization;
using Jiro.Commands.Results;

namespace Jiro.Commands;

[JsonDerivedType(typeof(GraphResult))]
[JsonDerivedType(typeof(TextResult))]
public interface ICommandResult
{
    string? Message { get; set; }
}
