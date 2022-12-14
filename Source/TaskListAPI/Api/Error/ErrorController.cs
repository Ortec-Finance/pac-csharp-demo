using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace TaskListAPI
{
    [OpenApiIgnore]
    public class ErrorController : BaseProtectedController
    {
        private readonly ILogger<ErrorController> _logger;
        public const string ErrorDev = "/error-dev";
        public const string ErrorProduction = "/error";

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route(ErrorProduction)]
        public IActionResult Error() => CreateError(false);

        [Route(ErrorDev)]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment env)
            => env.IsDevelopment() ? CreateError(true) : throw new InvalidOperationException();

        ObjectResult CreateError(bool isDevelopment)
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
            var error = feature.Error;

            var (statusCode, message) = ProcessError(error);
            if (isDevelopment)
            {
                return Problem(error.ToString(), null, statusCode, message);
            }

            if (statusCode < 500)
            {
                return Problem(null, null, statusCode, message);
            }

            _logger.LogError(LogEvents.InternalError, error, ResTaskListAPI.Server_error_);
            return Problem(null, null, statusCode);
        }

        (int StatusCode, string Message) ProcessError(Exception exception)
        {
            const int defaultStatusCode = (int)HttpStatusCode.InternalServerError;
            if (exception == null)
            {
                return (defaultStatusCode, "");
            }

            var responseException = exception as ResponseException;
            responseException ??= exception?.InnerException as ResponseException;
            if (responseException != null)
            {
                return (responseException.StatusCode, responseException.Message);
            }

            return (defaultStatusCode, GetExceptionMessages(exception!));
        }

        private static string GetExceptionMessages(Exception exception)
        {
            string result = exception.Message;
            if (exception.InnerException != null)
            {
                result += Environment.NewLine + GetExceptionMessages(exception.InnerException);
            }

            return result;
        }
    }
}