namespace Jiro.Commands.Tests.Unit.Results;

public class GraphResultTests
{
	[Fact]
	public void GraphResult_Create_WithRequiredParameters_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string message = "Test graph";
		var data = new[] { 1, 2, 3, 4, 5 };
		var units = new Dictionary<string, string> { { "x", "seconds" }, { "y", "value" } };

		// Act
		var result = GraphResult.Create(message, data, units);

		// Assert
		result.Should().NotBeNull();
		result.Message.Should().Be(message);
		result.Data.Should().Be(data);
		result.Units.Should().BeEquivalentTo(units);
		result.XAxis.Should().BeNull();
		result.YAxis.Should().BeNull();
		result.Note.Should().BeNull();
	}

	[Fact]
	public void GraphResult_Create_WithAllParameters_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string message = "Test graph";
		var data = new[] { 1, 2, 3 };
		var units = new Dictionary<string, string> { { "x", "time" }, { "y", "temperature" } };
		const string xAxis = "Time (seconds)";
		const string yAxis = "Temperature (°C)";
		const string note = "Test note";

		// Act
		var result = GraphResult.Create(message, data, units, xAxis, yAxis, note);

		// Assert
		result.Should().NotBeNull();
		result.Message.Should().Be(message);
		result.Data.Should().Be(data);
		result.Units.Should().BeEquivalentTo(units);
		result.XAxis.Should().Be(xAxis);
		result.YAxis.Should().Be(yAxis);
		result.Note.Should().Be(note);
	}

	[Fact]
	public void GraphResult_Create_WithNullData_ShouldCreateInstanceWithNullData()
	{
		// Arrange
		const string message = "Test";
		var units = new Dictionary<string, string>();

		// Act
		var result = GraphResult.Create(message, null, units);

		// Assert
		result.Data.Should().BeNull();
	}

	[Fact]
	public void GraphResult_Create_WithEmptyUnits_ShouldCreateInstanceWithEmptyUnits()
	{
		// Arrange
		const string message = "Test";
		var data = new[] { 1, 2 };
		var units = new Dictionary<string, string>();

		// Act
		var result = GraphResult.Create(message, data, units);

		// Assert
		result.Units.Should().BeEmpty();
	}

	[Fact]
	public void GraphResult_ShouldImplementICommandResult()
	{
		// Arrange
		var units = new Dictionary<string, string>();

		// Act
		var result = GraphResult.Create("test", null, units);

		// Assert
		result.Should().BeAssignableTo<ICommandResult>();
	}

	[Fact]
	public void GraphResult_Properties_ShouldBeSettable()
	{
		// Arrange
		var result = GraphResult.Create("test", null, new Dictionary<string, string>());
		const string newMessage = "Updated message";
		var newData = new[] { 10, 20 };

		// Act
		result.Message = newMessage;
		result.Data = newData;

		// Assert
		result.Message.Should().Be(newMessage);
		result.Data.Should().Be(newData);
	}
}
