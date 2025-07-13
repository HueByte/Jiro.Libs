namespace Jiro.Commands.Tests.Unit.Attributes;

public class CommandModuleAttributeTests
{
	[Fact]
	public void CommandModuleAttribute_WithModuleName_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string moduleName = "TestModule";

		// Act
		var attribute = new CommandModuleAttribute(moduleName);

		// Assert
		attribute.ModuleName.Should().Be(moduleName);
	}

	[Theory]
	[InlineData("Module1")]
	[InlineData("SampleModule")]
	[InlineData("ComplexModuleName")]
	[InlineData("")]
	public void CommandModuleAttribute_WithDifferentModuleNames_ShouldSetCorrectValues(string moduleName)
	{
		// Arrange & Act
		var attribute = new CommandModuleAttribute(moduleName);

		// Assert
		attribute.ModuleName.Should().Be(moduleName);
	}

	[Fact]
	public void CommandModuleAttribute_ShouldHaveCorrectAttributeUsage()
	{
		// Arrange
		var attributeType = typeof(CommandModuleAttribute);

		// Act
		var usageAttribute = attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), false).FirstOrDefault() as AttributeUsageAttribute;

		// Assert
		usageAttribute.Should().NotBeNull();
		usageAttribute!.ValidOn.Should().Be(AttributeTargets.Class);
	}
}
