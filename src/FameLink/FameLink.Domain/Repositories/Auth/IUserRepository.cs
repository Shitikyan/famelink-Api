using FameLink.Data.Models.Identity;
using FameLink.Domain.Responses.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FameLink.Domain.Repositories.Auth
{
    public interface IUserRepository
    {
        Task<FameLinkUser> Get(string userId);

        Task<List<FameLinkUser>> GetAll();

        IQueryable<FameLinkUser> GetAll(Expression<Func<FameLinkUser, bool>> predicate);

        Task<bool> IsInRole(FameLinkUser user, string role);

        Task<IdentityResult> Add(FameLinkUser user, string password, string role);

        Task Update(FameLinkUser user);

        Task<AccessToken> UpdateAndReturnUserToken(FameLinkUser user);

        Task Delete(FameLinkUser user);
    }
}
