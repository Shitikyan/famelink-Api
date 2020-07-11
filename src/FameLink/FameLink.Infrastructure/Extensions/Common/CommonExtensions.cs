using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FameLink.Infrastructure.Extensions.Common
{
    public static class CommonExtensions
    {
        public static string ToJson(this object obj)
        {
            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, };
            return JsonConvert.SerializeObject(obj, Formatting.None, settings);
        }

        public static IEnumerable<Claim> GetClaims(this ClaimsPrincipal principal, string type)
        {
            return principal.Claims.Where(q => q.Type == type);
        }

        public static string GetClaimValue(this ClaimsPrincipal principal, string type)
        {
            return principal.Claims.SingleOrDefault(q => q.Type == type)?.Value;
        }
    }
}
