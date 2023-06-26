using System.Reflection;
using Jiro.Commands.Attributes;
using Jiro.Commands.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Jiro.Commands.Base;

public static class CommandRegistrator
{
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
        };

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
