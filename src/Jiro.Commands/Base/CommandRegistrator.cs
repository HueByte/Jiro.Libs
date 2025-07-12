using System.Reflection;

using Jiro.Commands.Attributes;
using Jiro.Commands.Models;

using Microsoft.Extensions.DependencyInjection;

namespace Jiro.Commands.Base;

/// <summary>
/// Provides extension methods for registering command modules and commands with the dependency injection container.
/// </summary>
public static class CommandRegistrator
{
	/// <summary>
	/// Registers command modules and commands with the service collection.
	/// </summary>
	/// <param name="services">The service collection to register with.</param>
	/// <param name="defaultCommand">The name of the default command.</param>
	/// <returns>The updated <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection RegisterCommands(this IServiceCollection services, string defaultCommand)
	{
		var assemblies = ReflectionUtilities.GetDomainAssemblies() ?? throw new Exception("Assemblies is null, something went wrong");

		// types that contain CommandContainer attribute
		var commandModules = ReflectionUtilities.GetCommandModules(assemblies) ?? throw new Exception("Command modules is null, something went wrong");
		List<CommandModuleInfo> commandModulesInfos = new();

		foreach (var container in commandModules)
		{
			CommandModuleInfo commandModuleInfo = new();
			List<CommandInfo> commands = new();

			var preCommands = ReflectionUtilities.GetPotentialCommands(container);
			foreach (var methodInfo in preCommands)
			{
				var commandInfo = ReflectionUtilities.BuildCommandFromMethodInfo<ICommandBase, Task>(methodInfo);
				if (commandInfo is not null) commands.Add(commandInfo);
			}

			commandModuleInfo.SetCommands(commands);
			commandModuleInfo.SetName(container.GetCustomAttribute<CommandModuleAttribute>()?.ModuleName ?? "");

			services.AddScoped(container);
			commandModulesInfos.Add(commandModuleInfo);
		}
		;

		CommandsContext globalContainer = new();

		// add default command 
		globalContainer.SetDefaultCommand(defaultCommand.ToLower());
		globalContainer.AddModules(commandModulesInfos);
		globalContainer.AddCommands(commandModulesInfos
			.SelectMany(x => x.Commands.Select(e => e.Value))
			.ToList());

		services.AddSingleton(globalContainer);

		return services;
	}
}
