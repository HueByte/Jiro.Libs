namespace Jiro.Commands.Tests.Integration;

[CommandModule("TestModule")]
public class TestCommandModule : ICommandBase
{
	[Command("test-text", CommandType.Text, "test-text [message]", "A test text command")]
	public async Task<TextResult> TestTextCommand(string message = "Hello World")
	{
		await Task.CompletedTask; // Simulate async operation
		return TextResult.Create($"Test: {message}");
	}

	[Command("test-json", CommandType.Json, "test-json", "A test JSON command")]
	public async Task<JsonResult> TestJsonCommand()
	{
		await Task.CompletedTask; // Simulate async operation
		return JsonResult.Create(new { Status = "OK", Timestamp = DateTime.UtcNow });
	}

	[Command("test-graph", CommandType.Graph, "test-graph", "A test graph command")]
	public async Task<GraphResult> TestGraphCommand()
	{
		await Task.CompletedTask; // Simulate async operation
		var data = new[] { 1, 2, 3, 4, 5 };
		var units = new Dictionary<string, string> { { "x", "index" }, { "y", "value" } };
		return GraphResult.Create("Test Graph", data, units, "X Axis", "Y Axis", "Test note");
	}

	[Command("test-async", CommandType.Text, "test-async", "A test async command")]
	public async Task<TextResult> TestAsyncCommand()
	{
		await Task.Delay(10); // Simulate async work
		return TextResult.Create("Async completed");
	}

	[Command("test-with-params", CommandType.Text, "test-with-params <name> <age>", "A test command with parameters")]
	public async Task<TextResult> TestWithParameters(string name, int age)
	{
		await Task.CompletedTask; // Simulate async operation
		return TextResult.Create($"Name: {name}, Age: {age}");
	}
}
