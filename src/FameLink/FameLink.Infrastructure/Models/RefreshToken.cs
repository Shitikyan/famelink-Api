using System;

namespace FameLink.Infrastructure.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
