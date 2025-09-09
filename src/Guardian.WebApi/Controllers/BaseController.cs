using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Guardian.Application.Accounts.Contracts;
using Guardian.Application.Shared.Contracts;

namespace Guardian.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1")]
    [EnableRateLimiting("fixed")]
    public class BaseController : Controller
    {
        private IAuthenticatedUserService _authenticatedUser;
        protected IAuthenticatedUserService AuthenticatedUser => _authenticatedUser ??= HttpContext.RequestServices.GetService<IAuthenticatedUserService>();

        private ILoggerService _logger;
        protected ILoggerService Logger => _logger ??= HttpContext.RequestServices.GetService<ILoggerService>();
    }
}
