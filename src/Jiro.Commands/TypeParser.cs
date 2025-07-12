namespace Jiro.Commands
{
	/// <summary>
	/// Provides a base class for parsing string input into a specific type.
	/// </summary>
	public abstract class TypeParser
	{
		/// <summary>
		/// Parses the specified input string into an object of the target type.
		/// </summary>
		/// <param name="input">The input string to parse.</param>
		/// <returns>The parsed object, or null if parsing fails.</returns>
		public abstract object? Parse(string? input);
	}
}
