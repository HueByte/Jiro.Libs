using Microsoft.AspNetCore.Mvc;

namespace Jiro.Commands.Base
{
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Serves as the base API controller for command modules.
    /// </summary>
    public class BaseController : Controller { }
}