namespace Jiro.Commands.Tests.Unit.TypeParsers;

public class TypeParserTests
{
	private class TestTypeParser : TypeParser
	{
		public override object? Parse(string? input)
		{
			return input?.ToUpper();
		}
	}

	[Fact]
	public void TypeParser_ShouldBeAbstract()
	{
		// Arrange & Act & Assert
		typeof(TypeParser).Should().BeAbstract();
	}

	[Fact]
	public void TypeParser_Parse_ShouldBeOverridable()
	{
		// Arrange
		var parser = new TestTypeParser();

		// Act
		var result = parser.Parse("test");

		// Assert
		result.Should().Be("TEST");
	}

	[Fact]
	public void TypeParser_Parse_WithNull_ShouldHandleNull()
	{
		// Arrange
		var parser = new TestTypeParser();

		// Act
		var result = parser.Parse(null);

		// Assert
		result.Should().BeNull();
	}
}
