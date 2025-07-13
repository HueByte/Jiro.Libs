namespace Jiro.Commands.Tests.Unit.Base;

public class BaseControllerTests
{
	[Fact]
	public void BaseController_ShouldInheritFromController()
	{
		// Arrange & Act
		var controller = new BaseController();

		// Assert
		controller.Should().BeAssignableTo<Microsoft.AspNetCore.Mvc.Controller>();
	}

	[Fact]
	public void BaseController_ShouldHaveApiControllerAttribute()
	{
		// Arrange
		var controllerType = typeof(BaseController);

		// Act
		var hasApiControllerAttribute = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.ApiControllerAttribute), false).Any();

		// Assert
		hasApiControllerAttribute.Should().BeTrue();
	}

	[Fact]
	public void BaseController_ShouldHaveRouteAttribute()
	{
		// Arrange
		var controllerType = typeof(BaseController);

		// Act
		var routeAttribute = controllerType.GetCustomAttribute<Microsoft.AspNetCore.Mvc.RouteAttribute>();

		// Assert
		routeAttribute.Should().NotBeNull();
		routeAttribute!.Template.Should().Be("api/[controller]");
	}

	[Fact]
	public void BaseController_ShouldBeInstantiable()
	{
		// Arrange & Act
		var controller = new BaseController();

		// Assert
		controller.Should().NotBeNull();
	}
}
