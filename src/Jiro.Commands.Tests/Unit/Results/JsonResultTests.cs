namespace Jiro.Commands.Tests.Unit.Results;

public class JsonResultTests
{
	[Fact]
	public void JsonResult_Create_WithValidObject_ShouldCreateInstance()
	{
		// Arrange
		var testObject = new { Name = "Test", Value = 42 };

		// Act
		var result = JsonResult.Create(testObject);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeAssignableTo<ICommandResult>();
	}

	[Fact]
	public void JsonResult_Create_WithNullInput_ShouldThrowArgumentNullException()
	{
		// Arrange & Act
		Action act = () => JsonResult.Create<object>(null!);

		// Assert
		act.Should().Throw<ArgumentNullException>()
		   .WithParameterName("input")
		   .WithMessage("Input cannot be null*");
	}

	[Fact]
	public void JsonResult_Create_WithSimpleString_ShouldCreateInstance()
	{
		// Arrange
		const string input = "test string";

		// Act
		var result = JsonResult.Create(input);

		// Assert
		result.Should().NotBeNull();
	}

	[Fact]
	public void JsonResult_Create_WithComplexObject_ShouldCreateInstance()
	{
		// Arrange
		var complexObject = new
		{
			Id = 1,
			Name = "Test Object",
			Properties = new[] { "prop1", "prop2" },
			Nested = new { InnerValue = 123 }
		};

		// Act
		var result = JsonResult.Create(complexObject);

		// Assert
		result.Should().NotBeNull();
	}

	[Fact]
	public void JsonResult_Create_WithEmptyObject_ShouldCreateInstance()
	{
		// Arrange
		var emptyObject = new { };

		// Act
		var result = JsonResult.Create(emptyObject);

		// Assert
		result.Should().NotBeNull();
	}

	[Theory]
	[InlineData(42)]
	[InlineData(true)]
	[InlineData(3.14)]
	public void JsonResult_Create_WithPrimitiveTypes_ShouldCreateInstance<T>(T input)
	{
		// Arrange & Act
		var result = JsonResult.Create(input);

		// Assert
		result.Should().NotBeNull();
	}
}
