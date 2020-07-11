using FameLink.Common.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace FameLink.Infrastructure.Abstractions
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController<T> : ControllerBase
    {
        private ILogger<T> _logger;
        private IMediator _mediator;

        protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected Task<IActionResult> HandleRequest<TCommand>(object model, CancellationToken cToken = default)
            where TCommand : IBaseRequest
        {
            var command = model.Adapt<TCommand>();
            return HandleRequest(command, cToken);
        }

        protected async Task<IActionResult> HandleRequest(IBaseRequest command, CancellationToken cToken = default)
        {
            var result = (ResponseModel)await Mediator.Send(command, cToken);
            return StatusCode(result.StatusCodeValue, result.IsSuccessStatusCode ? result.GetItem() : result);
        }
    }
}
