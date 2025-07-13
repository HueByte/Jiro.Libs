namespace Jiro.Commands.Tests.Unit.Models;

using ParameterInfo = Jiro.Commands.Models.ParameterInfo;

public class ParameterInfoTests
{
	[Fact]
	public void ParameterInfo_Constructor_ShouldSetProperties()
	{
		// Arrange
		var type = typeof(string);
		var parser = new DefaultValueParser<string>();

		// Act
		var paramInfo = new ParameterInfo(type, parser);

		// Assert
		paramInfo.ParamType.Should().Be(type);
		paramInfo.Parser.Should().Be(parser);
	}

	[Fact]
	public void ParameterInfo_WithDifferentTypes_ShouldSetCorrectType()
	{
		// Arrange
		var intType = typeof(int);
		var boolType = typeof(bool);
		var parser = new DefaultValueParser<int>();

		// Act
		var intParam = new ParameterInfo(intType, parser);
		var boolParam = new ParameterInfo(boolType, parser);

		// Assert
		intParam.ParamType.Should().Be(intType);
		boolParam.ParamType.Should().Be(boolType);
	}

	[Fact]
	public void ParameterInfo_WithNullableTypes_ShouldSetCorrectType()
	{
		// Arrange
		var nullableIntType = typeof(int?);
		var parser = new DefaultValueParser<int?>();

		// Act
		var paramInfo = new ParameterInfo(nullableIntType, parser);

		// Assert
		paramInfo.ParamType.Should().Be(nullableIntType);
		paramInfo.Parser.Should().Be(parser);
	}

	[Fact]
	public void ParameterInfo_WithComplexTypes_ShouldSetCorrectType()
	{
		// Arrange
		var listType = typeof(List<string>);
		var parser = new DefaultValueParser<List<string>>();

		// Act
		var paramInfo = new ParameterInfo(listType, parser);

		// Assert
		paramInfo.ParamType.Should().Be(listType);
		paramInfo.Parser.Should().Be(parser);
	}
}
