using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExamplePlugin
{
    public static class PluginMiddlewareExtension
    {
        public static IApplicationBuilder UsePluginMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PluginMiddleware>();
        }
    }

    public class PluginMiddleware
    {
        private readonly RequestDelegate _next;

        public PluginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<PluginMiddleware> logger)
        {
            logger.LogInformation("Called from PluginMiddleware");

            await _next(context);
        }
    }
}