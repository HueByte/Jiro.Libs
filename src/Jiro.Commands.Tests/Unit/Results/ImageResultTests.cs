using Jiro.Core.Base.Results;

namespace Jiro.Commands.Tests.Unit.Results;

public class ImageResultTests
{
	[Fact]
	public void ImageResult_Constructor_WithMessage_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string message = "Test image result";

		// Act
		var result = new ImageResult(message);

		// Assert
		result.Should().NotBeNull();
		result.Message.Should().Be(message);
		result.Image.Should().BeNull();
		result.ImageUrl.Should().BeNull();
		result.Note.Should().BeNull();
	}

	[Fact]
	public void ImageResult_Constructor_WithAllParameters_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string message = "Test image";
		const string image = "base64imagedata";
		const string imageUrl = "https://example.com/image.jpg";
		const string note = "Test note";

		// Act
		var result = new ImageResult(message, image, imageUrl, note);

		// Assert
		result.Message.Should().Be(message);
		result.Image.Should().Be(image);
		result.ImageUrl.Should().Be(imageUrl);
		result.Note.Should().Be(note);
	}

	[Fact]
	public void ImageResult_Create_WithMessage_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string message = "Created image result";

		// Act
		var result = ImageResult.Create(message);

		// Assert
		result.Should().NotBeNull();
		result.Message.Should().Be(message);
		result.Image.Should().BeNull();
		result.ImageUrl.Should().BeNull();
		result.Note.Should().BeNull();
	}

	[Fact]
	public void ImageResult_Create_WithAllParameters_ShouldCreateCorrectInstance()
	{
		// Arrange
		const string message = "Created image";
		const string image = "imagedata123";
		const string imageUrl = "https://test.com/pic.png";
		const string note = "Creation note";

		// Act
		var result = ImageResult.Create(message, image, imageUrl, note);

		// Assert
		result.Message.Should().Be(message);
		result.Image.Should().Be(image);
		result.ImageUrl.Should().Be(imageUrl);
		result.Note.Should().Be(note);
	}

	[Fact]
	public void ImageResult_ShouldImplementICommandResult()
	{
		// Arrange & Act
		var result = new ImageResult("test");

		// Assert
		result.Should().BeAssignableTo<ICommandResult>();
	}

	[Fact]
	public void ImageResult_Properties_ShouldBeSettable()
	{
		// Arrange
		var result = new ImageResult("initial");
		const string newMessage = "Updated message";
		const string newImage = "newimagedata";
		const string newUrl = "https://new.com/image.gif";
		const string newNote = "Updated note";

		// Act
		result.Message = newMessage;
		result.Image = newImage;
		result.ImageUrl = newUrl;
		result.Note = newNote;

		// Assert
		result.Message.Should().Be(newMessage);
		result.Image.Should().Be(newImage);
		result.ImageUrl.Should().Be(newUrl);
		result.Note.Should().Be(newNote);
	}
}
