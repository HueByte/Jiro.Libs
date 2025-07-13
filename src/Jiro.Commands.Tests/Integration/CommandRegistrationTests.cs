namespace Jiro.Commands.Tests.Integration;

public class CommandRegistrationTests
{
	[Fact]
	public void RegisterCommands_ShouldRegisterTestModule()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		// Act
		services.RegisterCommands("test-text");
		var serviceProvider = services.BuildServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();

		// Assert
		commandsContext.Should().NotBeNull();
		commandsContext.DefaultCommand.Should().Be("test-text");
		commandsContext.Commands.Should().NotBeEmpty();
	}

	[Fact]
	public void RegisterCommands_ShouldRegisterAllCommandsFromTestModule()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		// Act
		services.RegisterCommands("test-text");
		var serviceProvider = services.BuildServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();

		// Assert
		commandsContext.Commands.Should().ContainKey("test-text");
		commandsContext.Commands.Should().ContainKey("test-json");
		commandsContext.Commands.Should().ContainKey("test-graph");
		commandsContext.Commands.Should().ContainKey("test-async");
		commandsContext.Commands.Should().ContainKey("test-with-params");
	}

	[Fact]
	public void RegisterCommands_ShouldRegisterModuleInServiceCollection()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		// Act
		services.RegisterCommands("test-text");
		var serviceProvider = services.BuildServiceProvider();

		// Assert
		var testModule = serviceProvider.GetService<TestCommandModule>();
		testModule.Should().NotBeNull();
	}

	[Fact]
	public void RegisterCommands_ShouldSetCorrectCommandTypes()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		// Act
		services.RegisterCommands("test-text");
		var serviceProvider = services.BuildServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();

		// Assert
		commandsContext.Commands["test-text"].CommandType.Should().Be(CommandType.Text);
		commandsContext.Commands["test-json"].CommandType.Should().Be(CommandType.Json);
		commandsContext.Commands["test-graph"].CommandType.Should().Be(CommandType.Graph);
	}

	[Fact]
	public void RegisterCommands_ShouldSetCorrectAsyncFlags()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		// Act
		services.RegisterCommands("test-text");
		var serviceProvider = services.BuildServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();

		// Assert
		commandsContext.Commands["test-text"].IsAsync.Should().BeTrue();
		commandsContext.Commands["test-async"].IsAsync.Should().BeTrue();
	}

	[Fact]
	public void RegisterCommands_ShouldSetCorrectCommandMetadata()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		// Act
		services.RegisterCommands("test-text");
		var serviceProvider = services.BuildServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();

		// Assert
		var testTextCommand = commandsContext.Commands["test-text"];
		testTextCommand.Name.Should().Be("test-text");
		testTextCommand.CommandSyntax.Should().Be("test-text [message]");
		testTextCommand.CommandDescription.Should().Be("A test text command");
	}
}
