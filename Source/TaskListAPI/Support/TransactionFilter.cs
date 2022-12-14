using System.Transactions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskListAPI
{
    public class TransactionFilter : IActionFilter
    {
        TransactionScope? _scope;
        public void OnActionExecuting(ActionExecutingContext context) => _scope = new TransactionScope();

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                _scope!.Complete();
            }

            _scope!.Dispose();
        }
    }
}