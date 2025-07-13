namespace Jiro.Commands.Tests.Unit.Results;

public class TextResultTests
{
	[Fact]
	public void TextResult_Create_WithMessage_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string message = "Test message";

		// Act
		var result = TextResult.Create(message);

		// Assert
		result.Should().NotBeNull();
		result.Message.Should().Be(message);
	}

	[Fact]
	public void TextResult_Create_WithNullMessage_ShouldCreateInstanceWithNullMessage()
	{
		// Arrange & Act
		var result = TextResult.Create(null);

		// Assert
		result.Should().NotBeNull();
		result.Message.Should().BeNull();
	}

	[Fact]
	public void TextResult_Create_WithEmptyMessage_ShouldCreateInstanceWithEmptyMessage()
	{
		// Arrange & Act
		var result = TextResult.Create(string.Empty);

		// Assert
		result.Should().NotBeNull();
		result.Message.Should().Be(string.Empty);
	}

	[Theory]
	[InlineData("Simple message")]
	[InlineData("Multi\nline\nmessage")]
	[InlineData("Message with special chars: !@#$%^&*()")]
	[InlineData("   Whitespace message   ")]
	public void TextResult_Create_WithVariousMessages_ShouldPreserveMessage(string message)
	{
		// Arrange & Act
		var result = TextResult.Create(message);

		// Assert
		result.Message.Should().Be(message);
	}

	[Fact]
	public void TextResult_ShouldImplementICommandResult()
	{
		// Arrange & Act
		var result = TextResult.Create("test");

		// Assert
		result.Should().BeAssignableTo<ICommandResult>();
	}
}
