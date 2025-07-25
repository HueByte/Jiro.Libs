using System.Linq.Expressions;
using System.Reflection;

using Jiro.Commands.Attributes;
using Jiro.Commands.Models;
using Jiro.Commands.TypeParsers;

namespace Jiro.Commands.Base
{
	/// <summary>
	/// Provides utility methods for reflection-based command discovery and invocation.
	/// </summary>
	internal static class ReflectionUtilities
	{
		/// <summary>
		/// Gets all assemblies loaded in the current application domain.
		/// </summary>
		/// <returns>An array of loaded assemblies.</returns>
		internal static Assembly[]? GetDomainAssemblies() => AppDomain.CurrentDomain.GetAssemblies();

		/// <summary>
		/// Gets all types marked as command modules from the provided assemblies.
		/// </summary>
		/// <param name="assemblies">The assemblies to search.</param>
		/// <returns>An array of types marked as command modules.</returns>
		internal static Type[]? GetCommandModules(Assembly[] assemblies)
		{
			var commandModules = assemblies
				.SelectMany(asm => asm.GetTypes()
					.Where(type =>
						!type.IsInterface
						&& type.GetCustomAttributes(typeof(CommandModuleAttribute), false).Length > 0
				))
				.ToArray();

			return commandModules;
		}

		/// <summary>
		/// Gets all methods marked as commands from the specified type.
		/// </summary>
		/// <param name="type">The type to search for command methods.</param>
		/// <returns>An array of methods marked as commands.</returns>
		internal static MethodInfo[] GetPotentialCommands(Type type)
		{
			var methodInfos = type
				.GetMethods()
				.Where(method => method.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
				.ToArray();

			return methodInfos;
		}

		/// <summary>
		/// Builds a <see cref="CommandInfo"/> object from the specified method info.
		/// </summary>
		/// <typeparam name="TBaseInstance">The base instance type.</typeparam>
		/// <typeparam name="TReturn">The return type.</typeparam>
		/// <param name="method">The method info to build from.</param>
		/// <returns>A <see cref="CommandInfo"/> object, or null if the method is invalid.</returns>
		internal static CommandInfo? BuildCommandFromMethodInfo<TBaseInstance, TReturn>(MethodInfo method)
		{
			if (method is null) return null;

			var delcaringType = method.DeclaringType;
			if (delcaringType is null) return null;

			var commandName = method.GetCustomAttribute<CommandAttribute>()?.CommandName.ToLower() ?? "";
			var commandType = method.GetCustomAttribute<CommandAttribute>()?.CommandType ?? CommandType.Text;
			var commandDescription = method.GetCustomAttribute<CommandAttribute>()?.CommandDescription ?? "";
			var commandSyntax = method.GetCustomAttribute<CommandAttribute>()?.CommandSyntax ?? "";
			// Check if method returns Task or Task<T>
			var isAsync = method.ReturnType == typeof(Task) ||
						  (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));

			var compiledMethod = CompileMethodInvoker<TBaseInstance, TReturn>(method);
			var args = GetParameters(method);
			if (compiledMethod is null) return null;

			// Create wrapper to match expected signature
			Func<ICommandBase, object?[], Task<ICommandResult?>> descriptor = async (instance, args) =>
			{
				var result = compiledMethod((TBaseInstance)(object)instance, args ?? Array.Empty<object?>());

				// All commands should return Tasks, so handle them accordingly
				if (result is Task task)
				{
					await task;

					// Use dynamic to access the Result property of Task<T>
					try
					{
						dynamic dynamicTask = task;
						var taskResult = dynamicTask.Result;
						if (taskResult is ICommandResult commandResult)
						{
							return commandResult;
						}
					}
					catch
					{
						// If dynamic access fails, the task likely didn't have a Result property (Task vs Task<T>)
					}
				}

				return null;
			};

			CommandInfo commandInfo = new(
				commandName,
				commandType,
				isAsync,
				delcaringType,
				descriptor,
				args,
				commandSyntax,
				commandDescription
			);
			return commandInfo;
		}

		/// <summary>
		/// Compiles a method into a delegate for fast invocation with the given instance and arguments.
		/// </summary>
		/// <typeparam name="TInstance">The type of the instance (usually the command module).</typeparam>
		/// <typeparam name="TReturn">The return type of the method.</typeparam>
		/// <param name="method">The method info to compile.</param>
		/// <returns>A delegate that invokes the method on the given instance with the provided arguments.</returns>
		internal static Func<TInstance, object?[], TReturn> CompileMethodInvoker<TInstance, TReturn>(MethodInfo method)
		{
			try
			{
				var parameters = method.GetParameters();
				var paramsExp = new Expression[parameters.Length];

				// set first param as Module instance that's fetched from DI container
				var instanceExp = Expression.Parameter(typeof(TInstance), "instance");
				var argsExp = Expression.Parameter(typeof(object?[]), "args");

				for (var i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i];

					var indexExp = Expression.Constant(i);
					var accessExp = Expression.ArrayIndex(argsExp, indexExp);
					paramsExp[i] = Expression.Convert(accessExp, parameter.ParameterType);
				}

				var callExp = Expression.Call(Expression.Convert(instanceExp, method.ReflectedType!), method, paramsExp);
				var finalExp = Expression.Convert(callExp, typeof(TReturn));
				var lambda = Expression.Lambda<Func<TInstance, object?[], TReturn>>(finalExp, instanceExp, argsExp);

				return lambda.Compile();
			}
			catch (Exception ex)
			{
				// Log the error or handle it appropriately
				throw new InvalidOperationException($"Failed to compile method invoker for {method.Name}: {ex.Message}", ex);
			}
		}

		/// <summary>
		/// Gets the list of parameter information for a given method, including type and parser.
		/// </summary>
		/// <param name="methodInfo">The method info to extract parameters from.</param>
		/// <returns>A read-only list of <see cref="ParameterInfo"/> objects for the method's parameters.</returns>
		internal static IReadOnlyList<Jiro.Commands.Models.ParameterInfo> GetParameters(MethodInfo methodInfo)
		{
			List<Jiro.Commands.Models.ParameterInfo> parameterInfos = new();

			var parameters = methodInfo.GetParameters();

			foreach (var parameter in parameters)
			{
				Jiro.Commands.Models.ParameterInfo parameterInfo = new(parameter.ParameterType, GetParser(parameter.ParameterType)!);
				parameterInfos.Add(parameterInfo);
			}

			return parameterInfos;
		}

		/// <summary>
		/// Gets a type parser for the specified parameter type.
		/// </summary>
		/// <param name="type">The parameter type to get a parser for.</param>
		/// <returns>A <see cref="TypeParser"/> for the given type.</returns>
		private static TypeParser? GetParser(Type type)
		{
			// todo
			return type switch
			{
				_ => (TypeParser)Activator.CreateInstance(typeof(DefaultValueParser<>).MakeGenericType(new Type[] { type }))!
			};
		}
	}
}
