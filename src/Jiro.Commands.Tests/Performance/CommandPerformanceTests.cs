using System.Diagnostics;

namespace Jiro.Commands.Tests.Performance;

public class CommandPerformanceTests
{
	private IServiceProvider SetupServiceProvider()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		services.RegisterCommands("test-text");
		return services.BuildServiceProvider();
	}

	[Fact]
	public async Task CommandExecution_ShouldCompleteWithinReasonableTime()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-text"];
		var tokens = new[] { "performance", "test" };
		var stopwatch = new Stopwatch();

		using var scope = serviceProvider.CreateScope();

		// Act
		stopwatch.Start();
		var result = await command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens);
		stopwatch.Stop();

		// Assert
		result.Should().NotBeNull();
		result.IsSuccess.Should().BeTrue();
		stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should complete within 1 second
	}

	[Fact]
	public async Task MultipleCommandExecutions_ShouldMaintainPerformance()
	{
		// Arrange
		var serviceProvider = SetupServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		var command = commandsContext.Commands["test-text"];
		var tokens = new[] { "bulk", "test" };
		const int iterations = 100;
		var stopwatch = new Stopwatch();

		using var scope = serviceProvider.CreateScope();

		// Act
		stopwatch.Start();
		var tasks = new List<Task<CommandResponse>>();
		for (int i = 0; i < iterations; i++)
		{
			tasks.Add(command.ExecuteAsync(scope.ServiceProvider, commandsContext, tokens));
		}
		await Task.WhenAll(tasks);
		stopwatch.Stop();

		// Assert
		tasks.All(t => t.Result.IsSuccess).Should().BeTrue();
		var averageTime = stopwatch.ElapsedMilliseconds / (double)iterations;
		averageTime.Should().BeLessThan(50); // Average should be less than 50ms per command
	}

	[Fact]
	public void TypeParser_StringParsing_ShouldBeEfficient()
	{
		// Arrange
		var parser = new DefaultValueParser<string>();
		const int iterations = 10000;
		const string testInput = "Performance test string";
		var stopwatch = new Stopwatch();

		// Act
		stopwatch.Start();
		for (int i = 0; i < iterations; i++)
		{
			parser.Parse(testInput);
		}
		stopwatch.Stop();

		// Assert
		var averageTime = stopwatch.ElapsedTicks / (double)iterations;
		averageTime.Should().BeLessThan(1000); // Should be very fast for string parsing
	}

	[Fact]
	public void TypeParser_IntegerParsing_ShouldBeEfficient()
	{
		// Arrange
		var parser = new DefaultValueParser<int>();
		const int iterations = 10000;
		const string testInput = "42";
		var stopwatch = new Stopwatch();

		// Act
		stopwatch.Start();
		for (int i = 0; i < iterations; i++)
		{
			parser.Parse(testInput);
		}
		stopwatch.Stop();

		// Assert
		var averageTime = stopwatch.ElapsedTicks / (double)iterations;
		averageTime.Should().BeLessThan(10000); // Should be reasonably fast for integer parsing
	}

	[Fact]
	public void CommandRegistration_ShouldCompleteQuickly()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();
		var stopwatch = new Stopwatch();

		// Act
		stopwatch.Start();
		services.RegisterCommands("test-text");
		var serviceProvider = services.BuildServiceProvider();
		var commandsContext = serviceProvider.GetRequiredService<CommandsContext>();
		stopwatch.Stop();

		// Assert
		commandsContext.Should().NotBeNull();
		stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Registration should complete within 5 seconds
	}
}
