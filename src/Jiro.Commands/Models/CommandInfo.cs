using System.Collections.Generic;
using System.Text;

using Jiro.Commands.Exceptions;
using Jiro.Commands.Results;

using Microsoft.Extensions.DependencyInjection;

namespace Jiro.Commands.Models;

/// <summary>
/// Represents metadata and execution logic for a command.
/// </summary>
public class CommandInfo
{
	/// <summary>
	/// Gets the name of the command.
	/// </summary>
	public string Name { get; } = string.Empty;

	/// <summary>
	/// Gets the type of the command.
	/// </summary>
	public CommandType CommandType { get; }

	/// <summary>
	/// Gets the syntax string for the command, if any.
	/// </summary>
	public string? CommandSyntax { get; }

	/// <summary>
	/// Gets the description of the command, if any.
	/// </summary>
	public string? CommandDescription { get; }

	/// <summary>
	/// Gets a value indicating whether the command is asynchronous.
	/// </summary>
	public bool IsAsync { get; } = false;

	/// <summary>
	/// Gets the type of the module that contains the command.
	/// </summary>
	public Type Module { get; } = default!;

	/// <summary>
	/// Gets the delegate that describes how to invoke the command.
	/// </summary>
	public Func<ICommandBase, object?[], Task<ICommandResult?>> Descriptor { get; private set; }

	/// <summary>
	/// Gets the list of parameters for the command.
	/// </summary>
	public IReadOnlyList<ParameterInfo?> Parameters { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="CommandInfo"/> class.
	/// </summary>
	/// <param name="name">The name of the command.</param>
	/// <param name="commandType">The type of the command.</param>
	/// <param name="isAsync">Whether the command is asynchronous.</param>
	/// <param name="container">The type of the module containing the command.</param>
	/// <param name="descriptor">The delegate describing how to invoke the command.</param>
	/// <param name="parameters">The list of parameters for the command.</param>
	/// <param name="commandSyntax">The syntax string for the command.</param>
	/// <param name="commandDescription">The description of the command.</param>
	public CommandInfo(string name, CommandType commandType, bool isAsync, Type container, Func<ICommandBase, object?[], Task<ICommandResult?>> descriptor, IReadOnlyList<ParameterInfo> parameters, string? commandSyntax, string? commandDescription)
	{
		Name = name;
		CommandType = commandType;
		IsAsync = isAsync;
		Module = container;
		Descriptor = descriptor;
		Parameters = parameters;
		CommandSyntax = commandSyntax;
		CommandDescription = commandDescription;
	}

	/// <summary>
	/// Executes the command asynchronously using the provided service provider and context.
	/// </summary>
	/// <param name="scopedServiceProvider">The scoped service provider for dependency injection.</param>
	/// <param name="commandModule">The command context module.</param>
	/// <param name="tokens">The input tokens for the command.</param>
	/// <returns>A <see cref="CommandResponse"/> representing the result of the command execution.</returns>
	public async Task<CommandResponse> ExecuteAsync(IServiceProvider scopedServiceProvider, CommandsContext commandModule, string[] tokens)
	{
		CommandResponse commandResult = new()
		{
			CommandName = Name,
			CommandType = CommandType,
			Result = null
		};

		var instance = scopedServiceProvider.GetRequiredService(Module);

		// use a quote-aware tokenizer instead of raw tokens
		var rawInput = string.Join(' ', tokens);
		var realTokens = Tokenize(rawInput);
		object?[] args = ParseArgs(commandModule, realTokens);

		if (instance is null)
		{
			commandResult.IsSuccess = false;
			commandResult.Result = TextResult.Create($"Couldn't create instance of {Name} command");

			return commandResult;
		}

		// All commands are async now
		commandResult.Result = await Descriptor((ICommandBase)instance, args);

		// If no result was returned, provide a default success message
		if (commandResult.Result == null)
		{
			commandResult.Result = TextResult.Create("Command executed successfully");
		}

		commandResult.IsSuccess = true;
		return commandResult;
	}

	/// <summary>
	/// Parses the input tokens into arguments for the command.
	/// </summary>
	/// <param name="commandModule">The command context module.</param>
	/// <param name="tokens">The input tokens for the command.</param>
	/// <returns>An array of parsed arguments.</returns>
	private object?[] ParseArgs(CommandsContext commandModule, string[] tokens)
	{
		object?[] args;

		if (Name == commandModule.DefaultCommand)
		{
			args = new object?[] { string.Join(' ', tokens) };
		}
		else if (Parameters is not null && tokens.Length > 0)
		{
			args = new object?[Parameters.Count];

			if (args.Length == 1 && Parameters[0]?.ParamType == typeof(string))
			{
				args[0] = string.Join(' ', tokens);
				return args;
			}

			for (int i = 0; i < Parameters.Count; i++)
			{
				var param = tokens.Length > i ? tokens[i] : null;
				if (Parameters[i]?.Parser is not null)
					args[i] = Parameters[i]?.Parser?.Parse(param);
			}
		}
		else
		{
			args = Array.Empty<object>();
		}

		return args;
	}

	// Quote-aware tokenizer
	private static string[] Tokenize(string input)
	{
		var tokens = new List<string>();
		var current = new StringBuilder();
		bool inQuotes = false;
		foreach (var c in input)
		{
			if (c == '"')
			{
				inQuotes = !inQuotes;
				if (!inQuotes && current.Length > 0)
				{
					tokens.Add(current.ToString());
					current.Clear();
				}
				continue;
			}
			if (char.IsWhiteSpace(c) && !inQuotes)
			{
				if (current.Length > 0)
				{
					tokens.Add(current.ToString());
					current.Clear();
				}
			}
			else
			{
				current.Append(c);
			}
		}
		if (current.Length > 0)
			tokens.Add(current.ToString());

		return tokens.ToArray();
	}
}
