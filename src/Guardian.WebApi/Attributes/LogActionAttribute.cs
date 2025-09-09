using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Guardian.Application.Accounts.Contracts;
using Guardian.Application.Shared.Contracts;
using System.Text.Json;

namespace Guardian.WebApi.Attributes
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class LogActionAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ILoggerService _logger;
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public LogActionAttribute(ILoggerService logger, IAuthenticatedUserService authenticatedUserService)
        {
            _logger = logger;
            _authenticatedUserService = authenticatedUserService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.ActionArguments;
            var executedContext = await next();
            object response = null;
            var userId = _authenticatedUserService?.Id.ToString() ?? "Anonymous";

            if (executedContext.Result is ObjectResult objectResult)
            {
                response = objectResult.Value;
            }

            _logger.LogInfo(new()
            {
                Action = context.ActionDescriptor.DisplayName,
                Message = "Executando ação do controlador.",
                Request = JsonSerializer.Serialize(request),
                Response = JsonSerializer.Serialize(response),
                UserId = userId
            }
            );
        }
    }
}