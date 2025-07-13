namespace Jiro.Commands.Tests.Unit.Attributes;

public class CommandAttributeTests
{
	[Fact]
	public void CommandAttribute_WithRequiredParameters_ShouldCreateCorrectInstance()
	{
		// Arrange & Act
		var attribute = new CommandAttribute("test-command");

		// Assert
		attribute.CommandName.Should().Be("test-command");
		attribute.CommandType.Should().Be(CommandType.Text);
		attribute.CommandSyntax.Should().Be("");
		attribute.CommandDescription.Should().Be("");
	}

	[Fact]
	public void CommandAttribute_WithAllParameters_ShouldCreateCorrectInstance()
	{
		// Arrange & Act
		var attribute = new CommandAttribute(
			"test-command",
			CommandType.Json,
			"syntax example",
			"This is a test command"
		);

		// Assert
		attribute.CommandName.Should().Be("test-command");
		attribute.CommandType.Should().Be(CommandType.Json);
		attribute.CommandSyntax.Should().Be("syntax example");
		attribute.CommandDescription.Should().Be("This is a test command");
	}

	[Theory]
	[InlineData("command1", CommandType.Text)]
	[InlineData("command2", CommandType.Json)]
	[InlineData("command3", CommandType.Graph)]
	[InlineData("command4", CommandType.Image)]
	public void CommandAttribute_WithDifferentCommandTypes_ShouldSetCorrectValues(string commandName, CommandType commandType)
	{
		// Arrange & Act
		var attribute = new CommandAttribute(commandName, commandType);

		// Assert
		attribute.CommandName.Should().Be(commandName);
		attribute.CommandType.Should().Be(commandType);
	}

	[Fact]
	public void CommandAttribute_WithNullSyntax_ShouldAcceptNull()
	{
		// Arrange & Act
		var attribute = new CommandAttribute("test", CommandType.Text, null, "description");

		// Assert
		attribute.CommandSyntax.Should().BeNull();
		attribute.CommandDescription.Should().Be("description");
	}
}
