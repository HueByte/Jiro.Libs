namespace Jiro.Commands.Tests.Integration;

public class CommandExecutionTests
{
	private IServiceProvider SetupServiceProvider()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		services.RegisterCommands("test-text");
		return services.BuildServiceProvider();
	}

	[Fact]
	public async Task ExecuteAsync_SimpleTextCommand_ShouldReturnCorrectResult()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-text"];
		var tokens = new[] { "Hello", "Test" };

		using var scope = serviceProvider.CreateScope();

		// Act
		var result = await command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens);

		// Assert
		result.Should().NotBeNull();
		result.CommandName.Should().Be("test-text");
		result.CommandType.Should().Be(CommandType.Text);
		result.IsSuccess.Should().BeTrue();
		result.Result.Should().BeOfType<TextResult>();
		((TextResult)result.Result!).Message.Should().Be("Test: Hello Test");
	}

	[Fact]
	public async Task ExecuteAsync_JsonCommand_ShouldReturnJsonResult()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-json"];
		var tokens = Array.Empty<string>();

		using var scope = serviceProvider.CreateScope();

		// Act
		var result = await command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens);

		// Assert
		result.Should().NotBeNull();
		result.CommandName.Should().Be("test-json");
		result.CommandType.Should().Be(CommandType.Json);
		result.IsSuccess.Should().BeTrue();
		result.Result.Should().BeOfType<JsonResult>();
	}

	[Fact]
	public async Task ExecuteAsync_GraphCommand_ShouldReturnGraphResult()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-graph"];
		var tokens = Array.Empty<string>();

		using var scope = serviceProvider.CreateScope();

		// Act
		var result = await command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens);

		// Assert
		result.Should().NotBeNull();
		result.CommandName.Should().Be("test-graph");
		result.CommandType.Should().Be(CommandType.Graph);
		result.IsSuccess.Should().BeTrue();
		result.Result.Should().BeOfType<GraphResult>();

		var graphResult = (GraphResult)result.Result!;
		graphResult.Message.Should().Be("Test Graph");
		graphResult.XAxis.Should().Be("X Axis");
		graphResult.YAxis.Should().Be("Y Axis");
		graphResult.Note.Should().Be("Test note");
	}

	[Fact]
	public async Task ExecuteAsync_AsyncCommand_ShouldExecuteCorrectly()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-async"];
		var tokens = Array.Empty<string>();

		using var scope = serviceProvider.CreateScope();

		// Act
		var result = await command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens);

		// Assert
		result.Should().NotBeNull();
		result.CommandName.Should().Be("test-async");
		result.IsSuccess.Should().BeTrue();
		result.Result.Should().BeOfType<TextResult>();
		((TextResult)result.Result!).Message.Should().Be("Async completed");
	}

	[Fact]
	public async Task ExecuteAsync_CommandWithParameters_ShouldParseParametersCorrectly()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-with-params"];
		var tokens = new[] { "John", "25" };

		using var scope = serviceProvider.CreateScope();

		// Act
		var result = await command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens);

		// Assert
		result.Should().NotBeNull();
		result.IsSuccess.Should().BeTrue();
		result.Result.Should().BeOfType<TextResult>();
		((TextResult)result.Result!).Message.Should().Be("Name: John, Age: 25");
	}

	[Fact]
	public async Task ExecuteAsync_WithQuotedParameters_ShouldTokenizeCorrectly()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-text"];
		var tokens = new[] { "\"Hello World\"", "from", "test" };

		using var scope = serviceProvider.CreateScope();

		// Act
		var result = await command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens);

		// Assert
		result.Should().NotBeNull();
		result.IsSuccess.Should().BeTrue();
		result.Result.Should().BeOfType<TextResult>();
		// The result should handle the quoted parameter correctly
		((TextResult)result.Result!).Message.Should().Contain("Hello World");
	}

	[Fact]
	public async Task ExecuteAsync_WithInvalidParameters_ShouldHandleError()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-with-params"];
		var tokens = new[] { "John", "invalid-age" }; // Invalid integer

		using var scope = serviceProvider.CreateScope();

		// Act & Assert
		await Assert.ThrowsAsync<CommandException>(() =>
			command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens));
	}
}
