namespace Jiro.Commands.Tests.Unit.TypeParsers;

public class DefaultValueParserTests
{
	[Fact]
	public void DefaultValueParser_String_ShouldParseCorrectly()
	{
		// Arrange
		var parser = new DefaultValueParser<string>();
		const string input = "test string";

		// Act
		var result = parser.Parse(input);

		// Assert
		result.Should().Be(input);
	}

	[Fact]
	public void DefaultValueParser_Int_ShouldParseCorrectly()
	{
		// Arrange
		var parser = new DefaultValueParser<int>();
		const string input = "42";

		// Act
		var result = parser.Parse(input);

		// Assert
		result.Should().Be(42);
	}

	[Fact]
	public void DefaultValueParser_Bool_ShouldParseCorrectly()
	{
		// Arrange
		var parser = new DefaultValueParser<bool>();

		// Act & Assert
		parser.Parse("true").Should().Be(true);
		parser.Parse("false").Should().Be(false);
		parser.Parse("True").Should().Be(true);
		parser.Parse("False").Should().Be(false);
	}

	[Fact]
	public void DefaultValueParser_Double_ShouldParseCorrectly()
	{
		// Arrange
		var parser = new DefaultValueParser<double>();
		const string input = "3.14";

		// Act
		var result = parser.Parse(input);

		// Assert
		result.Should().Be(3.14);
	}

	[Fact]
	public void DefaultValueParser_WithNullOrEmptyInput_ShouldReturnDefault()
	{
		// Arrange
		var intParser = new DefaultValueParser<int>();
		var stringParser = new DefaultValueParser<string>();

		// Act & Assert
		intParser.Parse(null).Should().Be(0);
		intParser.Parse("").Should().Be(0);
		intParser.Parse("   ").Should().Be(0);

		stringParser.Parse(null).Should().BeNull();
		stringParser.Parse("").Should().BeNull();
		stringParser.Parse("   ").Should().BeNull();
	}

	[Fact]
	public void DefaultValueParser_WithInvalidInput_ShouldThrowCommandException()
	{
		// Arrange
		var parser = new DefaultValueParser<int>();
		const string invalidInput = "not a number";

		// Act & Assert
		var action = () => parser.Parse(invalidInput);
		action.Should().Throw<CommandException>()
			  .WithMessage("*Couldn't parse 'not a number' as Int32*");
	}

	public enum TestEnum
	{
		Value1,
		Value2,
		Value3
	}

	[Fact]
	public void DefaultValueParser_Enum_ShouldParseCorrectly()
	{
		// Arrange
		var parser = new DefaultValueParser<TestEnum>();

		// Act & Assert
		parser.Parse("Value1").Should().Be(TestEnum.Value1);
		parser.Parse("value2").Should().Be(TestEnum.Value2); // Case insensitive
		parser.Parse("VALUE3").Should().Be(TestEnum.Value3);
	}

	[Fact]
	public void DefaultValueParser_Enum_WithInvalidValue_ShouldThrowCommandException()
	{
		// Arrange
		var parser = new DefaultValueParser<TestEnum>();

		// Act & Assert
		var action = () => parser.Parse("InvalidValue");
		action.Should().Throw<CommandException>();
	}

	[Fact]
	public void DefaultValueParser_NullableInt_ShouldParseCorrectly()
	{
		// Arrange
		var parser = new DefaultValueParser<int?>();

		// Act & Assert
		parser.Parse("42").Should().Be(42);
		parser.Parse(null).Should().BeNull();
		parser.Parse("").Should().BeNull();
	}

	[Fact]
	public void DefaultValueParser_DateTime_ShouldParseCorrectly()
	{
		// Arrange
		var parser = new DefaultValueParser<DateTime>();
		const string input = "2023-12-25";

		// Act
		var result = parser.Parse(input);

		// Assert
		result.Should().Be(new DateTime(2023, 12, 25));
	}

	[Fact]
	public void DefaultValueParser_Guid_ShouldParseCorrectly()
	{
		// Arrange
		var parser = new DefaultValueParser<Guid>();
		var guid = Guid.NewGuid();
		var input = guid.ToString();

		// Act
		var result = parser.Parse(input);

		// Assert
		result.Should().Be(guid);
	}
}
