using ExampleApp;
using Jiro.Commands.Base;
using Serilog;
using Serilog.Extensions.Logging;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var logger = new SerilogLoggerProvider(Log.Logger)
    .CreateLogger(nameof(Program));

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var configRef = builder.Configuration;

var modulePaths = configRef.GetSection("Modules").Get<string[]>();

PluginManager pluginManager = new(builder.Services, configRef, logger);

// build modules for dev purposes
pluginManager.BuildDevModules(modulePaths);

// load modules
pluginManager.LoadModuleAssemblies();

// add controllers from plugins
pluginManager.LoadModuleControllers();

// add configs and services from plugins
pluginManager.RegisterModuleServices();

// validate required modules
// pluginManager.ValidateModules(modulePaths);

var servicesRef = builder.Services;
servicesRef.AddControllers();
servicesRef.AddEndpointsApiExplorer();
servicesRef.AddSwaggerGen();

servicesRef.RegisterCommands(nameof(DefaultCommand.Default));


var app = builder.Build();

// register app extensions
pluginManager.RegisterAppExtensions(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
