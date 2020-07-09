using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace FameLink.Infrastructure.Abstractions
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController<T> : ControllerBase
    {
        private ILogger<T> _logger;

        protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();
    }
}
