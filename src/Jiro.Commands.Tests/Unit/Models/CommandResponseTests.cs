namespace Jiro.Commands.Tests.Unit.Models;

public class CommandResponseTests
{
	[Fact]
	public void CommandResponse_DefaultConstructor_ShouldHaveDefaultValues()
	{
		// Arrange & Act
		var response = new CommandResponse();

		// Assert
		response.CommandName.Should().BeNull();
		response.CommandType.Should().Be(default(CommandType));
		response.Result.Should().BeNull();
		response.IsSuccess.Should().BeFalse();
	}

	[Fact]
	public void CommandResponse_Properties_ShouldBeSettable()
	{
		// Arrange
		var response = new CommandResponse();
		const string commandName = "test-command";
		const CommandType commandType = CommandType.Json;
		var result = TextResult.Create("test result");
		const bool isSuccess = true;

		// Act
		response.CommandName = commandName;
		response.CommandType = commandType;
		response.Result = result;
		response.IsSuccess = isSuccess;

		// Assert
		response.CommandName.Should().Be(commandName);
		response.CommandType.Should().Be(commandType);
		response.Result.Should().Be(result);
		response.IsSuccess.Should().Be(isSuccess);
	}

	[Fact]
	public void CommandResponse_WithTextResult_ShouldSetCorrectly()
	{
		// Arrange
		var response = new CommandResponse();
		var textResult = TextResult.Create("Success message");

		// Act
		response.Result = textResult;
		response.IsSuccess = true;

		// Assert
		response.Result.Should().Be(textResult);
		response.IsSuccess.Should().BeTrue();
	}

	[Fact]
	public void CommandResponse_WithJsonResult_ShouldSetCorrectly()
	{
		// Arrange
		var response = new CommandResponse();
		var jsonResult = JsonResult.Create(new { status = "ok" });

		// Act
		response.Result = jsonResult;
		response.CommandType = CommandType.Json;

		// Assert
		response.Result.Should().Be(jsonResult);
		response.CommandType.Should().Be(CommandType.Json);
	}

	[Fact]
	public void CommandResponse_WithGraphResult_ShouldSetCorrectly()
	{
		// Arrange
		var response = new CommandResponse();
		var units = new Dictionary<string, string> { { "x", "time" }, { "y", "value" } };
		var graphResult = GraphResult.Create("Graph data", new[] { 1, 2, 3 }, units);

		// Act
		response.Result = graphResult;
		response.CommandType = CommandType.Graph;

		// Assert
		response.Result.Should().Be(graphResult);
		response.CommandType.Should().Be(CommandType.Graph);
	}

	[Theory]
	[InlineData("command1", CommandType.Text, true)]
	[InlineData("command2", CommandType.Json, false)]
	[InlineData("", CommandType.Graph, true)]
	[InlineData(null, CommandType.Image, false)]
	public void CommandResponse_WithVariousValues_ShouldSetCorrectly(string? commandName, CommandType commandType, bool isSuccess)
	{
		// Arrange & Act
		var response = new CommandResponse
		{
			CommandName = commandName,
			CommandType = commandType,
			IsSuccess = isSuccess
		};

		// Assert
		response.CommandName.Should().Be(commandName);
		response.CommandType.Should().Be(commandType);
		response.IsSuccess.Should().Be(isSuccess);
	}
}
