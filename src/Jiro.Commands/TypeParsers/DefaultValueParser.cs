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
            if (string.IsNullOrEmpty(input))
            {
                if (type.IsValueType)
                    return Activator.CreateInstance(type);

                return null;
            }

            return Convert.ChangeType(input, type);
        }
    }
}