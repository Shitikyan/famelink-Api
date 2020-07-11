using FameLink.Data.Models.Identity;
using FameLink.Domain.Responses.Auth;
using FameLink.Infrastructure.Abstractions;
using FameLink.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FameLink.Domain.Repositories.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<FameLinkUser> _userManager;
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IIdentityService _identityService;

        public UserRepository(
            UserManager<FameLinkUser> userManager,
            JwtTokenHandler jwtTokenHandler,
            IIdentityService identityService)
        {
            _userManager = userManager;
            _jwtTokenHandler = jwtTokenHandler;
            _identityService = identityService;
        }

        public Task<List<FameLinkUser>> GetAll()
        {
            return _userManager.Users.ToListAsync();
        }

        public Task<FameLinkUser> Get(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> Add(FameLinkUser user, string password, string role)
        {
            var identityResult = await _userManager.CreateAsync(user, password);
            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return identityResult;
        }

        public Task<bool> IsInRole(FameLinkUser user, string role)
        {
            return _userManager.IsInRoleAsync(user, role);
        }

        public Task Update(FameLinkUser user)
        {
            return _userManager.UpdateAsync(user);
        }

        public async Task<AccessToken> UpdateAndReturnUserToken(FameLinkUser user)
        {
            var token = await _jwtTokenHandler.GenerateToken(user);
            var refreshToken = _jwtTokenHandler.GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            return new AccessToken
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpireDate = refreshToken.ExpiresAt,
            };
        }

        public Task Delete(FameLinkUser user)
        {
            return _userManager.DeleteAsync(user);
        }

        public IQueryable<FameLinkUser> GetAll(Expression<Func<FameLinkUser, bool>> predicate)
        {
            return _userManager.Users.Where(predicate);
        }
    }
}
