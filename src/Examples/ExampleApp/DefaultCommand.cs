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
		public async Task<ICommandResult> Default()
		{
			await Task.CompletedTask; // Simulate async operation
			return TextResult.Create("Default command result");
		}
	}
}
