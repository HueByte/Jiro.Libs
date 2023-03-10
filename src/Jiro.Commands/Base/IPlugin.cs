using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jiro.Commands.Base
{
    public interface IPlugin
    {
        string PluginName { get; }
        void RegisterServices(IServiceCollection services);
        void RegisterAppConfigs(ConfigurationManager builder);
        void RegisterAppExtensions(IApplicationBuilder app);
    }
}