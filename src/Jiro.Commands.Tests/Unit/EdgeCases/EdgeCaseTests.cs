namespace Jiro.Commands.Tests.Unit.EdgeCases;

public class EdgeCaseTests
{
	[Fact]
	public void DefaultValueParser_WithNullInput_ShouldReturnDefaultForValueTypes()
	{
		// Arrange
		var intParser = new DefaultValueParser<int>();
		var boolParser = new DefaultValueParser<bool>();
		var doubleParser = new DefaultValueParser<double>();

		// Act & Assert
		intParser.Parse(null).Should().Be(0);
		boolParser.Parse(null).Should().Be(false);
		doubleParser.Parse(null).Should().Be(0.0);
	}

	[Fact]
	public void DefaultValueParser_WithNullInput_ShouldReturnNullForReferenceTypes()
	{
		// Arrange
		var stringParser = new DefaultValueParser<string>();

		// Act & Assert
		stringParser.Parse(null).Should().BeNull();
	}

	[Fact]
	public void DefaultValueParser_WithNullInput_ShouldReturnNullForNullableTypes()
	{
		// Arrange
		var nullableIntParser = new DefaultValueParser<int?>();
		var nullableBoolParser = new DefaultValueParser<bool?>();

		// Act & Assert
		nullableIntParser.Parse(null).Should().BeNull();
		nullableBoolParser.Parse(null).Should().BeNull();
	}

	[Fact]
	public void CommandAttribute_WithNullValues_ShouldHandleGracefully()
	{
		// Arrange & Act
		var attribute = new CommandAttribute("test", CommandType.Text, null, null);

		// Assert
		attribute.CommandName.Should().Be("test");
		attribute.CommandSyntax.Should().BeNull();
		attribute.CommandDescription.Should().BeNull();
	}

	[Fact]
	public void CommandModuleInfo_SetCommands_WithNullList_ShouldThrow()
	{
		// Arrange
		var moduleInfo = new CommandModuleInfo();

		// Act & Assert
		var action = () => moduleInfo.SetCommands(null!);
		action.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void CommandsContext_AddCommands_WithNullList_ShouldThrow()
	{
		// Arrange
		var context = new CommandsContext();

		// Act & Assert
		var action = () => context.AddCommands(null!);
		action.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void CommandResponse_WithNullResult_ShouldBeValid()
	{
		// Arrange & Act
		var response = new CommandResponse
		{
			CommandName = "test",
			CommandType = CommandType.Text,
			Result = null,
			IsSuccess = false
		};

		// Assert
		response.Result.Should().BeNull();
		response.IsSuccess.Should().BeFalse();
	}

	[Fact]
	public void TextResult_Create_WithVeryLongString_ShouldHandleCorrectly()
	{
		// Arrange
		var longString = new string('a', 100000); // 100k characters

		// Act
		var result = TextResult.Create(longString);

		// Assert
		result.Message.Should().Be(longString);
		result.Message!.Length.Should().Be(100000);
	}

	[Fact]
	public void GraphResult_Create_WithEmptyUnits_ShouldCreateValidResult()
	{
		// Arrange
		var emptyUnits = new Dictionary<string, string>();

		// Act
		var result = GraphResult.Create("test", null, emptyUnits);

		// Assert
		result.Units.Should().BeEmpty();
		result.Data.Should().BeNull();
	}

	[Fact]
	public void DefaultValueParser_WithWhitespaceOnlyInput_ShouldReturnDefault()
	{
		// Arrange
		var intParser = new DefaultValueParser<int>();
		var stringParser = new DefaultValueParser<string>();

		// Act & Assert
		intParser.Parse("   ").Should().Be(0);
		intParser.Parse("\t").Should().Be(0);
		intParser.Parse("\n").Should().Be(0);
		stringParser.Parse("   ").Should().BeNull();
	}

	[Fact]
	public void DefaultValueParser_WithUnicodeCharacters_ShouldHandleCorrectly()
	{
		// Arrange
		var stringParser = new DefaultValueParser<string>();
		const string unicodeString = "Hello 🌍 World 🚀";

		// Act
		var result = stringParser.Parse(unicodeString);

		// Assert
		result.Should().Be(unicodeString);
	}

	[Fact]
	public void CommandException_WithEmptyCommandName_ShouldBeValid()
	{
		// Arrange & Act
		var exception = new CommandException("", "Test message");

		// Assert
		exception.CommandName.Should().Be("");
		exception.Message.Should().Be("Test message");
	}

	[Fact]
	public void ParameterInfo_WithNullParser_ShouldAcceptNull()
	{
		// Arrange & Act
		var paramInfo = new Jiro.Commands.Models.ParameterInfo(typeof(string), null!);

		// Assert
		paramInfo.ParamType.Should().Be(typeof(string));
		paramInfo.Parser.Should().BeNull();
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("\t")]
	[InlineData("\n")]
	[InlineData("\r\n")]
	public void DefaultValueParser_WithVariousEmptyInputs_ShouldReturnDefault(string input)
	{
		// Arrange
		var parser = new DefaultValueParser<int>();

		// Act
		var result = parser.Parse(input);

		// Assert
		result.Should().Be(0);
	}
}
