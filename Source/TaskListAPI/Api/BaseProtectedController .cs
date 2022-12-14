using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TaskListAPI
{
    /// <summary>
    /// Implementing a base controller with all general functionality regarding
    /// validation and authorization ensures that it is not accidentally forgotten.
    /// </summary>
    // enforces Model.IsValid
    [ApiController]
    public abstract class BaseProtectedController : ControllerBase
    {
        protected IActionResult UpdateConditional(Func<IActionResult> update, ILogger logger)
        {
            return ConcurrencyHandlingUtils.UpdateConditional(
                update,
                statusCode => new StatusCodeResult(statusCode),
                logger);
        }
    }
}