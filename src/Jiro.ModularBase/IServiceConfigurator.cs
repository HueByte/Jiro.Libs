using Microsoft.Extensions.DependencyInjection;

namespace Jiro.Commands
{
    public interface IServiceConfigurator
    {
        string ConfiguratorName { get; }
        void RegisterServices(IServiceCollection services);
    }
}