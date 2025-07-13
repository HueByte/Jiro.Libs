namespace Jiro.Commands.Tests.Unit;

public class CommandTypeTests
{
	[Fact]
	public void CommandType_ShouldHaveExpectedValues()
	{
		// Arrange & Act
		var textValue = CommandType.Text;
		var jsonValue = CommandType.Json;
		var graphValue = CommandType.Graph;
		var imageValue = CommandType.Image;

		// Assert
		textValue.Should().Be(CommandType.Text);
		jsonValue.Should().Be(CommandType.Json);
		graphValue.Should().Be(CommandType.Graph);
		imageValue.Should().Be(CommandType.Image);
	}

	[Fact]
	public void CommandType_ShouldHaveCorrectUnderlyingValues()
	{
		// Assert
		((int)CommandType.Text).Should().Be(0);
		((int)CommandType.Json).Should().Be(1);
		((int)CommandType.Graph).Should().Be(2);
		((int)CommandType.Image).Should().Be(3);
	}
}
