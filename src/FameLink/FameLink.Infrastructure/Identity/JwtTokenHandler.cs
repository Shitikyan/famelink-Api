using FameLink.Data.Models.Identity;
using FameLink.Infrastructure.Models;
using FameLink.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FameLink.Infrastructure.Identity
{
    public class JwtTokenHandler
    {
        private readonly UserManager<FameLinkUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public JwtTokenHandler(UserManager<FameLinkUser> userManager, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<string> GenerateToken(FameLinkUser user)
        {
            string issuer = _jwtOptions.Issuer;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, issuer)
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim("role", role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddSeconds(_jwtOptions.ExpiresIn);

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetClaimsPrincipal(string accessToken)
        {
            var tokenTypeIndex = accessToken.IndexOf(' ');
            if (tokenTypeIndex != -1)
            {
                accessToken = accessToken.Substring(tokenTypeIndex + 1, accessToken.Length - tokenTypeIndex - 1);
            }

            var tokenValidationParameters = GetValidationParameters();
            tokenValidationParameters.ValidateLifetime = false;

            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken securityToken = null;
            try
            {
                principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
            }
            catch
            {
                return null;
            }

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }

        public bool IsTokenValid(string accessToken)
        {
            return GetClaimsPrincipal(accessToken) != null;
        }

        public TokenValidationParameters GetValidationParameters()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            string issuer = _jwtOptions.Issuer;
            bool validateIssuer = _jwtOptions.ValidateIssuer;

            return new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = issuer,
                ValidateIssuerSigningKey = _jwtOptions.ValidateIssuerSigningKey,
                ValidateIssuer = validateIssuer,
                ValidateAudience = validateIssuer,
            };
        }

        public RefreshToken GenerateRefreshToken()
        {
            var result = new RefreshToken();
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                result.Token = Convert.ToBase64String(randomNumber);
                result.ExpiresAt = DateTime.UtcNow.AddSeconds(_jwtOptions.RefreshExpiresIn);
            }

            return result;
        }
    }
}
