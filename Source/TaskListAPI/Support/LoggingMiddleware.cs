using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TaskListAPI
{
    public class LoggingMiddleware
    {
        readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation(LogEvents.BeginRequest,
                ResTaskListAPI.LoggingMiddleware_Invoke_Begin_request_ + context.Request.Path);
            await _next(context);
            _logger.LogInformation(LogEvents.EndRequest,
                ResTaskListAPI.LoggingMiddleware_Invoke_End_request_ + context.Request.Path);
        }
    }
}