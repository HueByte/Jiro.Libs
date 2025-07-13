# Jiro.Commands Tests

This project contains comprehensive tests for the Jiro.Commands library, including unit tests, integration tests, and performance tests.

## Test Structure

### Unit Tests (`Unit/`)

- **Attributes/**: Tests for `CommandAttribute` and `CommandModuleAttribute`
- **Results/**: Tests for all result types (`TextResult`, `JsonResult`, `GraphResult`, `ImageResult`)
- **TypeParsers/**: Tests for `TypeParser` and `DefaultValueParser<T>`
- **Exceptions/**: Tests for `CommandException`
- **Models/**: Tests for model classes (`CommandInfo`, `CommandResponse`, `ParameterInfo`, etc.)
- **Base/**: Tests for base classes like `BaseController`

### Integration Tests (`Integration/`)

- **CommandRegistrationTests**: Tests the command registration system
- **CommandExecutionTests**: Tests end-to-end command execution

### Performance Tests (`Performance/`)

- **CommandPerformanceTests**: Tests to ensure commands execute within reasonable time limits

### Test Helpers (`TestHelpers/`)

- **TestCommandModule**: A sample command module used for testing

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage" --settings test.runsettings
```

### Run Specific Test Categories

```bash
# Unit tests only
dotnet test --filter "FullyQualifiedName~Unit"

# Integration tests only
dotnet test --filter "FullyQualifiedName~Integration"

# Performance tests only
dotnet test --filter "FullyQualifiedName~Performance"
```

### Run Tests with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Test Coverage

The tests aim to provide comprehensive coverage of:

- ✅ All public APIs
- ✅ Command registration and discovery
- ✅ Command execution (sync and async)
- ✅ Parameter parsing and type conversion
- ✅ Result type handling
- ✅ Error handling and exceptions
- ✅ Performance characteristics
- ✅ Edge cases and boundary conditions

## Dependencies

- **xUnit**: Testing framework
- **FluentAssertions**: Fluent assertion library for more readable tests
- **Moq**: Mocking framework for unit tests
- **Microsoft.NET.Test.Sdk**: Test SDK
- **coverlet.collector**: Code coverage collector

## Test Conventions

1. **Naming**: `MethodUnderTest_StateUnderTest_ExpectedBehavior`
2. **Arrange-Act-Assert**: Each test follows the AAA pattern
3. **Theory vs Fact**: Use `[Theory]` for parameterized tests, `[Fact]` for single-case tests
4. **Descriptive assertions**: Use FluentAssertions for clear, readable assertions

## Example Test Structure

```csharp
[Fact]
public void CommandAttribute_WithMinimalParameters_ShouldCreateCorrectInstance()
{
    // Arrange
    const string commandName = "test-command";

    // Act
    var attribute = new CommandAttribute(commandName);

    // Assert
    attribute.CommandName.Should().Be(commandName);
    attribute.CommandType.Should().Be(CommandType.Text);
}
```
