using Jiro.Commands;
using Jiro.Commands.Attributes;
using Jiro.Commands.Results;

namespace ExampleApp
{
    [CommandModule("Default")]
    public class DefaultCommand : ICommandBase
    {
        public DefaultCommand()
        {
        }

        [Command("Default")]
        public Task<ICommandResult> Default()
        {
            return Task.FromResult((ICommandResult)TextResult.Create("Default command result"));
        }
    }
}