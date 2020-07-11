using Microsoft.AspNetCore.Http;
using System.Net;

namespace FameLink.Infrastructure.Extensions.Common
{
    public static class HttpExtensions
    {
        private const string GET = "GET";
        private const string POST = "POST";
        private const string PUT = "PUT";
        private const string DELETE = "DELETE";

        public static bool IsGetRequest(this HttpContext context)
        {
            return context.Request.Method == GET;
        }

        public static string GetClaimValue(this IHttpContextAccessor httpContextAccessor, string key) =>
            httpContextAccessor.HttpContext.User.GetClaimValue(key);

        public static bool CheckHeaderForValue(this HttpContext context, string key, string value)
        {
            var header = context.Request.Headers[key];
            return header == value;
        }

        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode) =>
            (int)statusCode >= 200 && (int)statusCode <= 299;
    }
}
