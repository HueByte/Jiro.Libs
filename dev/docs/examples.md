# Jiro.Libs Examples

This guide provides comprehensive examples showing how to use Jiro.Libs to build command-driven applications and plugins.

## Table of Contents

- [Getting Started](#getting-started)
- [Basic Command Module](#basic-command-module)
- [Advanced Command Examples](#advanced-command-examples)
- [Plugin Development](#plugin-development)
- [ASP.NET Core Integration](#aspnet-core-integration)
- [Custom Controllers](#custom-controllers)
- [Type Parsing Examples](#type-parsing-examples)
- [Real-World Examples](#real-world-examples)

## Getting Started

### Installation

First, install the Jiro.Commands NuGet package:

```bash
dotnet add package Jiro.Commands
```

### Basic Setup

Here's a minimal setup for an ASP.NET Core application with Jiro.Commands:

```csharp
using Jiro.Commands.Base;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.RegisterCommands("default"); // Register with default command

var app = builder.Build();

// Configure pipeline
app.UseRouting();
app.MapControllers();

app.Run();
```

## Basic Command Module

### Simple Text Command

Create a basic command module that responds with text:

```csharp
using Jiro.Commands;
using Jiro.Commands.Attributes;
using Jiro.Commands.Results;

[CommandModule("Greetings")]
public class GreetingCommands : ICommandBase
{
    [Command("hello", CommandType.Text, "hello [name]", "Greets a person")]
    public Task<ICommandResult> SayHello(string name = "World")
    {
        var message = $"Hello, {name}!";
        return Task.FromResult((ICommandResult)TextResult.Create(message));
    }

    [Command("goodbye", CommandType.Text, "goodbye [name]", "Says goodbye to a person")]
    public Task<ICommandResult> SayGoodbye(string name = "friend")
    {
        var message = $"Goodbye, {name}! See you later.";
        return Task.FromResult((ICommandResult)TextResult.Create(message));
    }
}
```

### JSON Response Command

Return structured data as JSON:

```csharp
using Jiro.Commands;
using Jiro.Commands.Attributes;
using Jiro.Commands.Results;

[CommandModule("Data")]
public class DataCommands : ICommandBase
{
    [Command("userinfo", CommandType.Json, "userinfo <userId>", "Gets user information")]
    public Task<ICommandResult> GetUserInfo(int userId)
    {
        var userData = new
        {
            Id = userId,
            Name = $"User{userId}",
            Email = $"user{userId}@example.com",
            CreatedAt = DateTime.UtcNow.AddDays(-userId * 10)
        };

        return Task.FromResult((ICommandResult)JsonResult.Create(userData));
    }

    [Command("stats", CommandType.Json, "stats", "Gets application statistics")]
    public Task<ICommandResult> GetStatistics()
    {
        var stats = new
        {
            TotalUsers = 1250,
            ActiveSessions = 45,
            LastUpdate = DateTime.UtcNow,
            Version = "3.0.0"
        };

        return Task.FromResult((ICommandResult)JsonResult.Create(stats));
    }
}
```

## Advanced Command Examples

### Async Operations with External Services

```csharp
using Jiro.Commands;
using Jiro.Commands.Attributes;
using Jiro.Commands.Results;

[CommandModule("Weather")]
public class WeatherCommands : ICommandBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WeatherCommands> _logger;

    public WeatherCommands(IHttpClientFactory httpClientFactory, ILogger<WeatherCommands> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [Command("weather", CommandType.Json, "weather <city>", "Gets weather information for a city")]
    public async Task<ICommandResult> GetWeather(string city)
    {
        try
        {
            _logger.LogInformation("Fetching weather for {City}", city);
            
            // Simulate API call
            await Task.Delay(500); // Simulate network delay
            
            var weatherData = new
            {
                City = city,
                Temperature = Random.Shared.Next(-10, 35),
                Condition = new[] { "Sunny", "Cloudy", "Rainy", "Snowy" }[Random.Shared.Next(4)],
                Humidity = Random.Shared.Next(30, 90),
                Timestamp = DateTime.UtcNow
            };

            return JsonResult.Create(weatherData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather for {City}", city);
            return TextResult.Create($"Error: Could not fetch weather for {city}");
        }
    }

    [Command("forecast", CommandType.Json, "forecast <city> [days]", "Gets weather forecast")]
    public async Task<ICommandResult> GetForecast(string city, int days = 5)
    {
        if (days < 1 || days > 14)
        {
            return TextResult.Create("Error: Days must be between 1 and 14");
        }

        await Task.Delay(300); // Simulate API call

        var forecast = Enumerable.Range(0, days)
            .Select(i => new
            {
                Date = DateTime.UtcNow.AddDays(i).ToString("yyyy-MM-dd"),
                Temperature = Random.Shared.Next(-5, 30),
                Condition = new[] { "Sunny", "Cloudy", "Rainy" }[Random.Shared.Next(3)]
            })
            .ToList();

        var result = new
        {
            City = city,
            Days = days,
            Forecast = forecast
        };

        return JsonResult.Create(result);
    }
}
```

### File Operations Command

```csharp
using Jiro.Commands;
using Jiro.Commands.Attributes;
using Jiro.Commands.Results;

[CommandModule("Files")]
public class FileCommands : ICommandBase
{
    private readonly IWebHostEnvironment _environment;

    public FileCommands(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [Command("listfiles", CommandType.Json, "listfiles [directory]", "Lists files in a directory")]
    public Task<ICommandResult> ListFiles(string directory = "")
    {
        try
        {
            var targetPath = string.IsNullOrWhiteSpace(directory) 
                ? _environment.ContentRootPath 
                : Path.Combine(_environment.ContentRootPath, directory);

            if (!Directory.Exists(targetPath))
            {
                return Task.FromResult((ICommandResult)TextResult.Create($"Directory not found: {directory}"));
            }

            var files = Directory.GetFiles(targetPath)
                .Select(f => new
                {
                    Name = Path.GetFileName(f),
                    Size = new FileInfo(f).Length,
                    Modified = File.GetLastWriteTime(f)
                })
                .ToList();

            var result = new
            {
                Directory = directory,
                FileCount = files.Count,
                Files = files
            };

            return Task.FromResult((ICommandResult)JsonResult.Create(result));
        }
        catch (Exception ex)
        {
            return Task.FromResult((ICommandResult)TextResult.Create($"Error: {ex.Message}"));
        }
    }

    [Command("fileinfo", CommandType.Json, "fileinfo <filename>", "Gets file information")]
    public Task<ICommandResult> GetFileInfo(string filename)
    {
        try
        {
            var filePath = Path.Combine(_environment.ContentRootPath, filename);
            
            if (!File.Exists(filePath))
            {
                return Task.FromResult((ICommandResult)TextResult.Create($"File not found: {filename}"));
            }

            var fileInfo = new FileInfo(filePath);
            var result = new
            {
                Name = fileInfo.Name,
                FullPath = fileInfo.FullName,
                Size = fileInfo.Length,
                Created = fileInfo.CreationTime,
                Modified = fileInfo.LastWriteTime,
                Extension = fileInfo.Extension,
                IsReadOnly = fileInfo.IsReadOnly
            };

            return Task.FromResult((ICommandResult)JsonResult.Create(result));
        }
        catch (Exception ex)
        {
            return Task.FromResult((ICommandResult)TextResult.Create($"Error: {ex.Message}"));
        }
    }
}
```

## Plugin Development

### Creating a Plugin

Create a complete plugin with configuration, services, and commands:

**1. Plugin Interface Implementation (`PluginMain.cs`)**

```csharp
using Jiro.Commands.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyCustomPlugin;

public class PluginMain : IPlugin
{
    public string PluginName { get; } = "MyCustomPlugin";

    public void RegisterAppConfigs(ConfigurationManager builder)
    {
        // Add plugin-specific configuration
        builder.AddJsonFile("mycustomplugin.json", optional: true, reloadOnChange: true);
    }

    public void RegisterAppExtensions(IApplicationBuilder app)
    {
        // Add custom middleware
        app.UseCustomPluginMiddleware();
    }

    public void RegisterServices(IServiceCollection services)
    {
        // Register plugin services
        services.AddScoped<IMyPluginService, MyPluginService>();
        services.AddSingleton<IPluginCache, PluginCache>();
    }
}
```

**2. Plugin Service (`MyPluginService.cs`)**

```csharp
public interface IMyPluginService
{
    Task<string> ProcessDataAsync(string input);
    Task<List<string>> GetCachedItemsAsync();
}

public class MyPluginService : IMyPluginService
{
    private readonly ILogger<MyPluginService> _logger;
    private readonly IPluginCache _cache;

    public MyPluginService(ILogger<MyPluginService> logger, IPluginCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task<string> ProcessDataAsync(string input)
    {
        _logger.LogInformation("Processing data: {Input}", input);
        
        // Simulate processing
        await Task.Delay(100);
        
        var result = $"Processed: {input.ToUpper()}";
        await _cache.SetAsync($"processed_{input}", result);
        
        return result;
    }

    public async Task<List<string>> GetCachedItemsAsync()
    {
        return await _cache.GetAllKeysAsync();
    }
}
```

**3. Plugin Commands (`PluginCommands.cs`)**

```csharp
using Jiro.Commands;
using Jiro.Commands.Attributes;
using Jiro.Commands.Results;

[CommandModule("MyPlugin")]
public class PluginCommands : ICommandBase
{
    private readonly IMyPluginService _pluginService;
    private readonly ILogger<PluginCommands> _logger;

    public PluginCommands(IMyPluginService pluginService, ILogger<PluginCommands> logger)
    {
        _pluginService = pluginService;
        _logger = logger;
    }

    [Command("process", CommandType.Text, "process <input>", "Processes input data")]
    public async Task<ICommandResult> ProcessData(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return TextResult.Create("Error: Input cannot be empty");
        }

        try
        {
            var result = await _pluginService.ProcessDataAsync(input);
            return TextResult.Create(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing data");
            return TextResult.Create("Error: Failed to process data");
        }
    }

    [Command("cache", CommandType.Json, "cache", "Gets cached items")]
    public async Task<ICommandResult> GetCachedItems()
    {
        try
        {
            var items = await _pluginService.GetCachedItemsAsync();
            return JsonResult.Create(new { CachedItems = items, Count = items.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached items");
            return TextResult.Create("Error: Failed to retrieve cached items");
        }
    }

    [Command("status", CommandType.Json, "status", "Gets plugin status")]
    public Task<ICommandResult> GetStatus()
    {
        var status = new
        {
            PluginName = "MyCustomPlugin",
            Version = "1.0.0",
            Status = "Active",
            Uptime = DateTime.UtcNow.ToString("O"),
            ServicesRegistered = 2
        };

        return Task.FromResult((ICommandResult)JsonResult.Create(status));
    }
}
```

## ASP.NET Core Integration

### Complete Program.cs Setup

```csharp
using MyApp.Commands;
using Jiro.Commands.Base;
using Serilog;

// Configure logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add logging
    builder.Host.UseSerilog();

    // Add services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpClient();

    // Plugin Manager Setup
    var configRef = builder.Configuration;
    var modulePaths = configRef.GetSection("Modules").Get<string[]>();
    
    var logger = Log.ForContext<Program>();
    PluginManager pluginManager = new(builder.Services, configRef, logger);

    // Build and load plugins in development
    if (builder.Environment.IsDevelopment())
    {
        pluginManager.BuildDevModules(modulePaths);
    }

    // Load plugin assemblies
    pluginManager.LoadModuleAssemblies();
    pluginManager.LoadModuleControllers();
    pluginManager.RegisterModuleServices();

    // Register commands with default command
    builder.Services.RegisterCommands("help");

    var app = builder.Build();

    // Register plugin app extensions
    pluginManager.RegisterAppExtensions(app);

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    // Add custom endpoint for command execution
    app.MapPost("/api/commands/execute", async (ExecuteCommandRequest request, IServiceProvider serviceProvider) =>
    {
        // Command execution logic here
        return Results.Ok();
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

### Configuration (appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Modules": [
    "ExamplePlugin",
    "WeatherPlugin",
    "FileManagerPlugin"
  ],
  "AllowedHosts": "*"
}
```

## Custom Controllers

### Plugin Controller with API Endpoints

```csharp
using Jiro.Commands.Base;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PluginController : BaseController
{
    private readonly IMyPluginService _pluginService;

    public PluginController(IMyPluginService pluginService)
    {
        _pluginService = pluginService;
    }

    [HttpGet("test")]
    public IActionResult PluginTest()
    {
        return Ok(new { Message = "Plugin Controller Executed", Timestamp = DateTime.UtcNow });
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessData([FromBody] ProcessDataRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Input))
        {
            return BadRequest("Input is required");
        }

        var result = await _pluginService.ProcessDataAsync(request.Input);
        return Ok(new { Result = result });
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new 
        { 
            Status = "Healthy",
            Plugin = "MyCustomPlugin",
            Timestamp = DateTime.UtcNow 
        });
    }
}

public class ProcessDataRequest
{
    public string Input { get; set; } = string.Empty;
}
```

## Type Parsing Examples

### Custom Parameter Types

```csharp
[CommandModule("TypeExamples")]
public class TypeExampleCommands : ICommandBase
{
    [Command("calculate", CommandType.Text, "calculate <operation> <x> <y>", "Performs calculations")]
    public Task<ICommandResult> Calculate(string operation, double x, double y)
    {
        var result = operation.ToLower() switch
        {
            "add" => x + y,
            "subtract" => x - y,
            "multiply" => x * y,
            "divide" => y != 0 ? x / y : double.NaN,
            _ => double.NaN
        };

        var message = double.IsNaN(result) 
            ? $"Invalid operation: {operation}" 
            : $"{operation} {x} {y} = {result}";

        return Task.FromResult((ICommandResult)TextResult.Create(message));
    }

    [Command("dateinfo", CommandType.Json, "dateinfo <date>", "Gets date information")]
    public Task<ICommandResult> GetDateInfo(DateTime date)
    {
        var info = new
        {
            Date = date.ToString("yyyy-MM-dd"),
            DayOfWeek = date.DayOfWeek.ToString(),
            DayOfYear = date.DayOfYear,
            WeekOfYear = GetWeekOfYear(date),
            IsWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday,
            Quarter = (date.Month - 1) / 3 + 1
        };

        return Task.FromResult((ICommandResult)JsonResult.Create(info));
    }

    [Command("search", CommandType.Json, "search <query> [maxResults]", "Searches with optional limit")]
    public Task<ICommandResult> Search(string query, int maxResults = 10)
    {
        // Simulate search results
        var results = Enumerable.Range(1, Math.Min(maxResults, 50))
            .Select(i => new
            {
                Id = i,
                Title = $"Result {i} for '{query}'",
                Score = Random.Shared.NextDouble(),
                Url = $"https://example.com/result{i}"
            })
            .OrderByDescending(r => r.Score)
            .ToList();

        var response = new
        {
            Query = query,
            MaxResults = maxResults,
            TotalFound = results.Count,
            Results = results
        };

        return Task.FromResult((ICommandResult)JsonResult.Create(response));
    }

    private static int GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        return culture.Calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
    }
}
```

### Boolean and Array Parameters

```csharp
[CommandModule("Advanced")]
public class AdvancedCommands : ICommandBase
{
    [Command("backup", CommandType.Text, "backup <path> [compress]", "Creates a backup")]
    public Task<ICommandResult> CreateBackup(string path, bool compress = false)
    {
        // Simulate backup operation
        var message = compress 
            ? $"Creating compressed backup of {path}..." 
            : $"Creating backup of {path}...";

        return Task.FromResult((ICommandResult)TextResult.Create(message));
    }

    [Command("average", CommandType.Text, "average <numbers...>", "Calculates average of numbers")]
    public Task<ICommandResult> CalculateAverage(params double[] numbers)
    {
        if (numbers.Length == 0)
        {
            return Task.FromResult((ICommandResult)TextResult.Create("Error: No numbers provided"));
        }

        var average = numbers.Average();
        var message = $"Average of [{string.Join(", ", numbers)}] = {average:F2}";

        return Task.FromResult((ICommandResult)TextResult.Create(message));
    }
}
```

## Real-World Examples

### Database Operations Command

```csharp
[CommandModule("Database")]
public class DatabaseCommands : ICommandBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseCommands> _logger;

    public DatabaseCommands(IServiceScopeFactory scopeFactory, ILogger<DatabaseCommands> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    [Command("dbstatus", CommandType.Json, "dbstatus", "Gets database status")]
    public async Task<ICommandResult> GetDatabaseStatus()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Simulate database check
            await Task.Delay(100);

            var status = new
            {
                Status = "Connected",
                LastCheck = DateTime.UtcNow,
                // UserCount = await dbContext.Users.CountAsync(),
                // ActiveConnections = dbContext.Database.GetDbConnection().State.ToString()
                UserCount = 1250,
                ActiveConnections = "Open"
            };

            return JsonResult.Create(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database status");
            return TextResult.Create("Error: Database connection failed");
        }
    }
}
```

### System Information Command

```csharp
[CommandModule("System")]
public class SystemCommands : ICommandBase
{
    [Command("sysinfo", CommandType.Json, "sysinfo", "Gets system information")]
    public Task<ICommandResult> GetSystemInfo()
    {
        var sysInfo = new
        {
            Environment = new
            {
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                Is64BitOS = Environment.Is64BitOperatingSystem,
                WorkingSet = Environment.WorkingSet,
                Version = Environment.Version.ToString()
            },
            Process = new
            {
                Id = Environment.ProcessId,
                StartTime = DateTime.UtcNow, // Approximate
                WorkingMemory = GC.GetTotalMemory(false),
                ThreadCount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count
            },
            Runtime = new
            {
                FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                RuntimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
                Architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString()
            }
        };

        return Task.FromResult((ICommandResult)JsonResult.Create(sysInfo));
    }

    [Command("gc", CommandType.Text, "gc", "Triggers garbage collection")]
    public Task<ICommandResult> TriggerGarbageCollection()
    {
        var beforeMemory = GC.GetTotalMemory(false);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var afterMemory = GC.GetTotalMemory(true);
        
        var freed = beforeMemory - afterMemory;
        var message = $"Garbage collection completed. Freed: {freed:N0} bytes";

        return Task.FromResult((ICommandResult)TextResult.Create(message));
    }
}
```

## Best Practices

### Error Handling

```csharp
[CommandModule("ErrorHandling")]
public class ErrorHandlingCommands : ICommandBase
{
    private readonly ILogger<ErrorHandlingCommands> _logger;

    public ErrorHandlingCommands(ILogger<ErrorHandlingCommands> logger)
    {
        _logger = logger;
    }

    [Command("safeoperation", CommandType.Text, "safeoperation <input>", "Demonstrates safe error handling")]
    public async Task<ICommandResult> SafeOperation(string input)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(input))
            {
                return TextResult.Create("Error: Input cannot be empty");
            }

            if (input.Length > 100)
            {
                return TextResult.Create("Error: Input too long (max 100 characters)");
            }

            // Simulate operation that might fail
            if (input.ToLower() == "error")
            {
                throw new InvalidOperationException("Simulated error");
            }

            await Task.Delay(100); // Simulate work

            return TextResult.Create($"Operation completed successfully for: {input}");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided: {Input}", input);
            return TextResult.Create($"Error: Invalid argument - {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in SafeOperation with input: {Input}", input);
            return TextResult.Create("Error: An unexpected error occurred");
        }
    }
}
```

### Performance Monitoring

```csharp
[CommandModule("Performance")]
public class PerformanceCommands : ICommandBase
{
    private readonly ILogger<PerformanceCommands> _logger;

    public PerformanceCommands(ILogger<PerformanceCommands> logger)
    {
        _logger = logger;
    }

    [Command("benchmark", CommandType.Json, "benchmark <iterations>", "Runs a simple benchmark")]
    public async Task<ICommandResult> RunBenchmark(int iterations = 1000)
    {
        if (iterations < 1 || iterations > 1000000)
        {
            return TextResult.Create("Error: Iterations must be between 1 and 1,000,000");
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Simple CPU-bound operation
        var sum = 0L;
        for (int i = 0; i < iterations; i++)
        {
            sum += i * i;
            if (i % 10000 == 0)
            {
                await Task.Yield(); // Allow other tasks to run
            }
        }

        stopwatch.Stop();

        var result = new
        {
            Iterations = iterations,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            IterationsPerSecond = iterations / (stopwatch.ElapsedMilliseconds / 1000.0),
            Result = sum
        };

        _logger.LogInformation("Benchmark completed: {Iterations} iterations in {ElapsedMs}ms", 
            iterations, stopwatch.ElapsedMilliseconds);

        return JsonResult.Create(result);
    }
}
```

---

## Summary

This examples guide covers:

1. **Basic Setup**: How to configure Jiro.Commands in an ASP.NET Core application
2. **Command Creation**: From simple text responses to complex async operations
3. **Plugin Development**: Complete plugin architecture with services and controllers
4. **Type Handling**: Working with different parameter types and validation
5. **Real-World Scenarios**: Database operations, system monitoring, and error handling
6. **Best Practices**: Error handling, logging, and performance considerations

Each example is self-contained and demonstrates specific features of the Jiro.Commands framework. You can use these as starting points for your own command modules and plugins.

For more detailed information, see:

- [Command Creation Pipeline](command-creation-pipeline.md)
- [Compiled Lambdas](compiled-lambdas.md)
- [API Reference](~/api/)
