using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TaskListAPI
{
    public static class ConcurrencyHandlingUtils
    {
        // For all post, put & delete statements to handle concurrency exceptions:
        public static IActionResult UpdateConditional(Func<IActionResult> update, Func<int, IActionResult> statusCode,
            ILogger logger)
        {
            try
            {
                return update();
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException)
            {
                logger.LogWarning(LogEvents.DatabaseConcurrencyConflict, ResTaskListAPI.Database_concurrency_error_);
                return statusCode((int)HttpStatusCode.Conflict);
            }
        }
    }
}