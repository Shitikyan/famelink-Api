using FameLink.Domain.Repositories.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FameLink.Domain.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }

        void SaveChanges();

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
