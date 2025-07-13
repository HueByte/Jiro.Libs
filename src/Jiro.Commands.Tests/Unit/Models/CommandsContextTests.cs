namespace Jiro.Commands.Tests.Unit.Models;

public class CommandsContextTests
{
	[Fact]
	public void CommandsContext_DefaultConstructor_ShouldHaveDefaultValues()
	{
		// Arrange & Act
		var context = new CommandsContext();

		// Assert
		context.DefaultCommand.Should().Be(string.Empty);
		context.CommandModules.Should().BeEmpty();
		context.Commands.Should().BeEmpty();
	}

	[Fact]
	public void CommandsContext_SetDefaultCommand_ShouldSetCorrectValue()
	{
		// Arrange
		var context = new CommandsContext();
		const string defaultCommand = "help";

		// Act
		context.SetDefaultCommand(defaultCommand);

		// Assert
		context.DefaultCommand.Should().Be(defaultCommand);
	}

	[Fact]
	public void CommandsContext_AddCommands_ShouldAddToCommandsCollection()
	{
		// Arrange
		var context = new CommandsContext();
		var commands = CreateMockCommands();

		// Act
		context.AddCommands(commands);

		// Assert
		context.Commands.Should().HaveCount(2);
		context.Commands.Should().ContainKey("command1");
		context.Commands.Should().ContainKey("command2");
	}

	[Fact]
	public void CommandsContext_AddModules_ShouldAddToModulesCollection()
	{
		// Arrange
		var context = new CommandsContext();
		var modules = CreateMockModules();

		// Act
		context.AddModules(modules);

		// Assert
		context.CommandModules.Should().HaveCount(2);
		context.CommandModules.Should().ContainKey("Module1");
		context.CommandModules.Should().ContainKey("Module2");
	}

	[Fact]
	public void CommandsContext_AddCommands_WithDuplicateNames_ShouldNotOverwrite()
	{
		// Arrange
		var context = new CommandsContext();
		var firstCommand = CreateMockCommand("duplicate", CommandType.Text);
		var secondCommand = CreateMockCommand("duplicate", CommandType.Json);
		var commands = new List<CommandInfo> { firstCommand, secondCommand };

		// Act
		context.AddCommands(commands);

		// Assert
		context.Commands.Should().HaveCount(1);
		context.Commands["duplicate"].Should().Be(firstCommand);
	}

	[Fact]
	public void CommandsContext_AddModules_WithDuplicateNames_ShouldNotOverwrite()
	{
		// Arrange
		var context = new CommandsContext();
		var firstModule = CreateMockModule("DuplicateModule");
		var secondModule = CreateMockModule("DuplicateModule");
		var modules = new List<CommandModuleInfo> { firstModule, secondModule };

		// Act
		context.AddModules(modules);

		// Assert
		context.CommandModules.Should().HaveCount(1);
		context.CommandModules["DuplicateModule"].Should().Be(firstModule);
	}

	[Fact]
	public void CommandsContext_AddCommands_WithEmptyList_ShouldNotAddAnyCommands()
	{
		// Arrange
		var context = new CommandsContext();
		var emptyCommands = new List<CommandInfo>();

		// Act
		context.AddCommands(emptyCommands);

		// Assert
		context.Commands.Should().BeEmpty();
	}

	[Fact]
	public void CommandsContext_AddModules_WithEmptyList_ShouldNotAddAnyModules()
	{
		// Arrange
		var context = new CommandsContext();
		var emptyModules = new List<CommandModuleInfo>();

		// Act
		context.AddModules(emptyModules);

		// Assert
		context.CommandModules.Should().BeEmpty();
	}

	[Fact]
	public void CommandsContext_MultipleOperations_ShouldMaintainState()
	{
		// Arrange
		var context = new CommandsContext();
		const string defaultCommand = "default";
		var commands = CreateMockCommands();
		var modules = CreateMockModules();

		// Act
		context.SetDefaultCommand(defaultCommand);
		context.AddCommands(commands);
		context.AddModules(modules);

		// Assert
		context.DefaultCommand.Should().Be(defaultCommand);
		context.Commands.Should().HaveCount(2);
		context.CommandModules.Should().HaveCount(2);
	}

	private static List<CommandInfo> CreateMockCommands()
	{
		return new List<CommandInfo>
		{
			CreateMockCommand("command1", CommandType.Text),
			CreateMockCommand("command2", CommandType.Json)
		};
	}

	private static List<CommandModuleInfo> CreateMockModules()
	{
		return new List<CommandModuleInfo>
		{
			CreateMockModule("Module1"),
			CreateMockModule("Module2")
		};
	}

	private static CommandInfo CreateMockCommand(string name, CommandType type)
	{
		var mockDescriptor = new Func<ICommandBase, object?[], Task<ICommandResult?>>((_, _) => Task.FromResult<ICommandResult?>(null));
		var mockParameters = new List<Jiro.Commands.Models.ParameterInfo>().AsReadOnly();

		return new CommandInfo(
			name,
			type,
			false,
			typeof(object),
			mockDescriptor,
			mockParameters,
			null,
			null
		);
	}

	private static CommandModuleInfo CreateMockModule(string name)
	{
		var module = new CommandModuleInfo();
		module.SetName(name);
		return module;
	}
}
