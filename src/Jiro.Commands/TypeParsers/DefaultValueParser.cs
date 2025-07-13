using System;
using System.ComponentModel;
using System.Globalization;

using Jiro.Commands.Exceptions;

namespace Jiro.Commands.TypeParsers
{
	/// <summary>
	/// Provides a default implementation of <see cref="TypeParser"/> for parsing values of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The target type to parse to.</typeparam>
	public class DefaultValueParser<T> : TypeParser
	{
		/// <summary>
		/// Parses the specified input string into an object of type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="input">The input string to parse.</param>
		/// <returns>The parsed object, or the default value if input is null or empty.</returns>
		public override object? Parse(string? input)
		{
			var type = typeof(T);
			var underlyingType = Nullable.GetUnderlyingType(type);
			var targetType = underlyingType ?? type;
			var isNullable = underlyingType is not null;

			if (string.IsNullOrWhiteSpace(input))
			{
				if (isNullable)
					return null;
				if (targetType.IsValueType)
					return Activator.CreateInstance(targetType);
				return null;
			}

			try
			{
				if (targetType.IsEnum)
					return Enum.Parse(targetType, input, ignoreCase: true);

				var converter = TypeDescriptor.GetConverter(targetType);
				if (converter != null && converter.CanConvertFrom(typeof(string)))
					return converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

				return Convert.ChangeType(input, targetType, CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				throw new CommandException(targetType.Name,
					$"Couldn't parse '{input}' as {targetType.Name}: {ex.Message}");
			}
		}
	}
}
