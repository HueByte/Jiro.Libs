using System;
using System.Text.Json;

namespace Jiro.Commands.Results;

/// <summary>
/// Represents a JSON result for a command.
/// </summary>
public class JsonResult : ICommandResult
{
	/// <summary>
	/// Gets or sets the JSON message of the result.
	/// </summary>
	public string? Message { get; set; }

	private JsonResult(string jsonData)
	{
		Message = jsonData;
	}

	/// <summary>
	/// Creates a new <see cref="JsonResult"/> instance from the specified input object.
	/// </summary>
	/// <typeparam name="T">The type of the input object.</typeparam>
	/// <param name="input">The object to serialize to JSON.</param>
	/// <returns>A new <see cref="JsonResult"/> instance.</returns>
	/// <exception cref="ArgumentNullException">Thrown if input is null.</exception>
	/// <exception cref="InvalidOperationException">Thrown if serialization results in an empty JSON string.</exception>
	public static JsonResult Create<T>(T input)
	{
		if (input == null)
		{
			throw new ArgumentNullException(nameof(input), "Input cannot be null");
		}

		string jsonData = JsonSerializer.Serialize(input);

		if (string.IsNullOrEmpty(jsonData))
		{
			throw new InvalidOperationException("Serialization resulted in an empty JSON string");
		}

		return new JsonResult(jsonData);
	}

	/// <summary>
	/// Creates a new <see cref="JsonResult"/> instance from the specified JSON string.
	/// </summary>
	/// <param name="jsonData">The JSON string to use as the message.</param
	/// <returns>A new <see cref="JsonResult"/> instance.</returns>
	/// <exception cref="ArgumentNullException">Thrown if jsonData is null or empty.</exception>
	/// <exception cref="InvalidOperationException">Thrown if jsonData is not a valid JSON
	/// string.</exception>
	/// <remarks>
	/// This method is useful when you already have a JSON string that you want to use as
	/// the result message.
	/// </remarks>
	public static JsonResult Create(string jsonData)
	{
		if (string.IsNullOrEmpty(jsonData))
		{
			throw new ArgumentNullException(nameof(jsonData), "JSON data cannot be null or empty");
		}

		return new JsonResult(jsonData);
	}
}
