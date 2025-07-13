namespace Jiro.Commands.Tests.Unit.Exceptions;

public class CommandExceptionTests
{
	[Fact]
	public void CommandException_WithCommandNameAndMessage_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string commandName = "test-command";
		const string message = "Test error message";

		// Act
		var exception = new CommandException(commandName, message);

		// Assert
		exception.CommandName.Should().Be(commandName);
		exception.Message.Should().Be(message);
	}

	[Fact]
	public void CommandException_ShouldInheritFromException()
	{
		// Arrange & Act
		var exception = new CommandException("test", "message");

		// Assert
		exception.Should().BeAssignableTo<Exception>();
	}

	[Fact]
	public void CommandException_CommandName_ShouldBeSettable()
	{
		// Arrange
		var exception = new CommandException("initial", "message");
		const string newCommandName = "updated-command";

		// Act
		exception.CommandName = newCommandName;

		// Assert
		exception.CommandName.Should().Be(newCommandName);
	}

	[Theory]
	[InlineData("command1", "Error in command1")]
	[InlineData("complex-command-name", "Complex error message with details")]
	[InlineData("", "Empty command name")]
	public void CommandException_WithVariousInputs_ShouldSetPropertiesCorrectly(string commandName, string message)
	{
		// Arrange & Act
		var exception = new CommandException(commandName, message);

		// Assert
		exception.CommandName.Should().Be(commandName);
		exception.Message.Should().Be(message);
	}

	[Fact]
	public void CommandException_WhenThrown_ShouldBeCatchable()
	{
		// Arrange
		const string commandName = "failing-command";
		const string message = "This command failed";

		// Act & Assert
		Action throwAction = () => throw new CommandException(commandName, message);

		throwAction.Should().Throw<CommandException>()
				   .Where(ex => ex.CommandName == commandName)
				   .And.Message.Should().Be(message);
	}
}
