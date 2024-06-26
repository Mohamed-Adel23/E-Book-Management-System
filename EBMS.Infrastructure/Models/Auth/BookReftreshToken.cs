using Microsoft.EntityFrameworkCore;
namespace EBMS.Infrastructure.Models.Auth
{
    [Owned]
    public class BookRefreshToken
    {
        public string Token { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Expires_at { get; set; }
        public DateTime? Revoked_at { get; set; }

        public bool IsExpired => DateTime.Now > Expires_at;
        public bool IsActive => Revoked_at == null && !IsExpired;
    }
}
