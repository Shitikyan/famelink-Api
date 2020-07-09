namespace FameLink.Infrastructure.Options
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public bool SaveToken { get; set; }
        public string Issuer { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateIssuer { get; set; }
        public int ExpiresIn { get; set; }
        public int RefreshExpiresIn { get; set; }
    }
}
