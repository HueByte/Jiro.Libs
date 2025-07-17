using Jiro.Commands;

namespace Jiro.Commands.Results
{
	/// <summary>
	/// Represents an image result for a command.
	/// </summary>
	public sealed class ImageResult : ICommandResult
	{
		/// <summary>
		/// Gets or sets the message of the result.
		/// </summary>
		public string? Message { get; set; }

		/// <summary>
		/// Gets or sets an optional note for the result.
		/// </summary>
		public string? Note { get; set; }

		/// <summary>
		/// Gets or sets the URL of the image.
		/// </summary>
		public string? ImageUrl { get; set; }

		/// <summary>
		/// Gets or sets the image data as a string.
		/// </summary>
		public string? Image { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageResult"/> class.
		/// </summary>
		/// <param name="message">The message for the result.</param>
		/// <param name="image">The image data as a string.</param>
		/// <param name="imageUrl">The URL of the image.</param>
		/// <param name="note">An optional note for the result.</param>
		public ImageResult(string message, string? image = null, string? imageUrl = null, string? note = null)
		{
			Message = message;
			Image = image;
			ImageUrl = imageUrl;
			Note = note;
		}

		/// <summary>
		/// Creates a new <see cref="ImageResult"/> instance with the specified parameters.
		/// </summary>
		/// <param name="message">The message for the result.</param>
		/// <param name="image">The image data as a string.</param>
		/// <param name="imageUrl">The URL of the image.</param>
		/// <param name="note">An optional note for the result.</param>
		/// <returns>A new <see cref="ImageResult"/> instance.</returns>
		public static ImageResult Create(string message, string? image = null, string? imageUrl = null, string? note = null)
		{
			return new ImageResult(message, image, imageUrl, note);
		}
	}
}
