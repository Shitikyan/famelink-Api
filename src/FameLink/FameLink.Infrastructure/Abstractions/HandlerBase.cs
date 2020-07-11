using Microsoft.Extensions.Logging;

namespace FameLink.Infrastructure.Abstractions
{
    public abstract class HandlerBase<TBase>
    {
        protected readonly ILogger<TBase> _logger;
        protected readonly IIdentityService _identityService;

        protected HandlerBase(ILogger<TBase> logger, IIdentityService identityService)
        {
            _logger = logger;
            _identityService = identityService;
        }

        protected string UserId => _identityService.GetUserId();
        protected string Email => _identityService.GetUserEmail();
    }
}
