using System.Text.Json.Serialization;
namespace EBMS.Infrastructure.Models.Auth
{
    public class BookAuthModel
    {
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expires_at { get; set; }
        // It won't serialized to response body, but it will in the response cookie
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
