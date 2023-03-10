using Jiro.Commands.Base;
using Microsoft.AspNetCore.Mvc;

namespace ExamplePlugin
{
    public class PluginController : BaseController
    {
        public PluginController()
        {

        }

        [HttpGet("PluginTest")]
        public IActionResult PluginTest()
        {
            return Ok("Plugin Controller Executed");
        }
    }
}