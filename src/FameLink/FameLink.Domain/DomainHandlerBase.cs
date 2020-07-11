using FameLink.Domain.UnitOfWork;
using FameLink.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;

namespace FameLink.Domain
{
    public abstract class DomainHandlerBase<TBase> : HandlerBase<TBase>
    {
        protected IUnitOfWork _unitOfWork;

        protected DomainHandlerBase(
            ILogger<TBase> logger,
            IUnitOfWork unitOfWork,
            IIdentityService identityService) : base(logger, identityService)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
