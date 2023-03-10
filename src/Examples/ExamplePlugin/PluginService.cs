using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExamplePlugin
{
    public interface IPluginService
    {
        string ServiceTest();
    }

    public class PluginService : IPluginService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public PluginService(ILogger<PluginService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string ServiceTest()
        {
            _logger.LogInformation("Called from PluginService");

            return $"Plugin Service Executed {_configuration["TestValue"]}";
        }
    }
}