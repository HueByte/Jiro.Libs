namespace Jiro.Commands.Tests.Unit.Models;

public class CommandModuleInfoTests
{
	[Fact]
	public void CommandModuleInfo_DefaultConstructor_ShouldHaveDefaultValues()
	{
		// Arrange & Act
		var moduleInfo = new CommandModuleInfo();

		// Assert
		moduleInfo.Name.Should().Be(string.Empty);
		moduleInfo.Commands.Should().BeEmpty();
	}

	[Fact]
	public void CommandModuleInfo_SetName_ShouldSetCorrectName()
	{
		// Arrange
		var moduleInfo = new CommandModuleInfo();
		const string moduleName = "TestModule";

		// Act
		moduleInfo.SetName(moduleName);

		// Assert
		moduleInfo.Name.Should().Be(moduleName);
	}

	[Fact]
	public void CommandModuleInfo_SetCommands_ShouldAddCommandsToCollection()
	{
		// Arrange
		var moduleInfo = new CommandModuleInfo();
		var mockCommands = CreateMockCommands();

		// Act
		moduleInfo.SetCommands(mockCommands);

		// Assert
		moduleInfo.Commands.Should().HaveCount(2);
		moduleInfo.Commands.Should().ContainKey("command1");
		moduleInfo.Commands.Should().ContainKey("command2");
	}

	[Fact]
	public void CommandModuleInfo_SetCommands_WithDuplicateNames_ShouldNotOverwrite()
	{
		// Arrange
		var moduleInfo = new CommandModuleInfo();
		var firstCommand = CreateMockCommand("duplicate", CommandType.Text);
		var secondCommand = CreateMockCommand("duplicate", CommandType.Json);
		var commands = new List<CommandInfo> { firstCommand, secondCommand };

		// Act
		moduleInfo.SetCommands(commands);

		// Assert
		moduleInfo.Commands.Should().HaveCount(1);
		moduleInfo.Commands["duplicate"].Should().Be(firstCommand); // First one wins
	}

	[Fact]
	public void CommandModuleInfo_SetCommands_WithEmptyList_ShouldNotAddAnyCommands()
	{
		// Arrange
		var moduleInfo = new CommandModuleInfo();
		var emptyCommands = new List<CommandInfo>();

		// Act
		moduleInfo.SetCommands(emptyCommands);

		// Assert
		moduleInfo.Commands.Should().BeEmpty();
	}

	[Fact]
	public void CommandModuleInfo_SetCommands_MultipleTimes_ShouldAccumulateCommands()
	{
		// Arrange
		var moduleInfo = new CommandModuleInfo();
		var firstBatch = new List<CommandInfo> { CreateMockCommand("cmd1", CommandType.Text) };
		var secondBatch = new List<CommandInfo> { CreateMockCommand("cmd2", CommandType.Json) };

		// Act
		moduleInfo.SetCommands(firstBatch);
		moduleInfo.SetCommands(secondBatch);

		// Assert
		moduleInfo.Commands.Should().HaveCount(2);
		moduleInfo.Commands.Should().ContainKey("cmd1");
		moduleInfo.Commands.Should().ContainKey("cmd2");
	}

	private static List<CommandInfo> CreateMockCommands()
	{
		return new List<CommandInfo>
		{
			CreateMockCommand("command1", CommandType.Text),
			CreateMockCommand("command2", CommandType.Json)
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
}
