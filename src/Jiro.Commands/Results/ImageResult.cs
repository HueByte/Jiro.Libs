using Jiro.Commands;

namespace Jiro.Core.Base.Results
{
    public sealed class ImageResult : ICommandResult
    {
        public string? Message { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
        public string? Image { get; set; }

        public ImageResult(string message, string? image = null, string? imageUrl = null, string? note = null)
        {
            Message = message;
            Image = image;
            ImageUrl = imageUrl;
            Note = note;
        }

        public static ImageResult Create(string message, string? image = null, string? imageUrl = null, string? note = null)
        {
            return new ImageResult(message, image, imageUrl, note);
        }
    }
}