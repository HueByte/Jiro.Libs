using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jiro.Commands.Base
{
	/// <summary>
	/// Represents a plugin that can be registered with the application.
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// Gets the name of the plugin.
		/// </summary>
		string PluginName { get; }

		/// <summary>
		/// Registers services required by the plugin.
		/// </summary>
		/// <param name="services">The service collection to register with.</param>
		void RegisterServices(IServiceCollection services);

		/// <summary>
		/// Registers application configuration for the plugin.
		/// </summary>
		/// <param name="builder">The configuration manager to register with.</param>
		void RegisterAppConfigs(ConfigurationManager builder);

		/// <summary>
		/// Registers application extensions for the plugin.
		/// </summary>
		/// <param name="app">The application builder to register with.</param>
		void RegisterAppExtensions(IApplicationBuilder app);
	}
}
