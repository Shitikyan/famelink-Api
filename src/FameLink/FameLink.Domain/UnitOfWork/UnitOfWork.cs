using FameLink.Data.Models.Identity;
using FameLink.Domain.Repositories.Auth;
using FameLink.Infrastructure.Abstractions;
using FameLink.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FameLink.Domain.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private readonly DbContext _context;
        private readonly UserManager<FameLinkUser> _userManager;
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IIdentityService _identityService;
        private bool _disposed;

        public UnitOfWork(DbContext context,
                          UserManager<FameLinkUser> userManager,
                          JwtTokenHandler jwtTokenHandler,
                          IIdentityService identityService)
        {
            _context = context;
            _userManager = userManager;
            _jwtTokenHandler = jwtTokenHandler;
            _identityService = identityService;
        }

        private IUserRepository _users;
        public IUserRepository Users => _users ??= new UserRepository(_userManager, _jwtTokenHandler, _identityService);

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _disposed = true;
            _context.Dispose();
        }

        ~UnitOfWork()
        {
            if (!_disposed)
            {
                Dispose();
            }
        }
    }
}
