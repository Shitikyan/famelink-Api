using FameLink.Data.Models.Identity;
using FameLink.Infrastructure.Abstractions;
using FameLink.Infrastructure.Extensions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Elevant.Vailability.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<FameLinkUser> _userManager;

        public IdentityService(IHttpContextAccessor context, UserManager<FameLinkUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string GetUserId()
        {
            return _context.GetClaimValue(JwtRegisteredClaimNames.Sub);
        }

        public string GetUserName()
        {
            return _context.HttpContext.User.Identity.Name;
        }

        public string GetUserEmail()
        {
            return _context.GetClaimValue(JwtRegisteredClaimNames.Email);
        }

        public IIdentity GetCurrentUser()
        {
            return _context.HttpContext.User.Identity;
        }

        public IEnumerable<string> GetUserRoles()
        {
            var roles = _context.HttpContext.User.GetClaims(ClaimsIdentity.DefaultRoleClaimType);
            return roles.Select(r => r.Value);
        }
    }
}
