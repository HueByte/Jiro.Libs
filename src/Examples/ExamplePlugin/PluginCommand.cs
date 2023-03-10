using Jiro.Commands;
using Jiro.Commands.Attributes;
using Jiro.Commands.Results;

namespace ExamplePlugin
{
    [CommandModule("PluginCommand")]
    public class PluginCommand : ICommandBase
    {
        private readonly IPluginService _pluginService;
        public PluginCommand(IPluginService pluginService)
        {
            _pluginService = pluginService;
        }

        [Command("PluginTest", commandSyntax: "PluginTest", commandDescription: "Tests plugin command")]
        public async Task<ICommandResult> PluginTest()
        {
            _pluginService.ServiceTest();

            await Task.Delay(1000);
            return TextResult.Create("Plugin Command Executed");
        }
    }
}