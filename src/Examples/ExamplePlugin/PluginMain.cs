using Jiro.Commands.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExamplePlugin;

public class PluginMain : IPlugin
{
    public string PluginName { get; } = "PluginMain";

    // optional
    public void RegisterAppConfigs(ConfigurationManager builder)
    {
        builder.AddJsonFile("example.config.json", optional: true, reloadOnChange: true);
    }

    // optional
    public void RegisterAppExtensions(IApplicationBuilder app)
    {
        app.UsePluginMiddleware();
    }

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IPluginService, PluginService>();
    }
}