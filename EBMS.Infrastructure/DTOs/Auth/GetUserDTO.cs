using System.Text.Json.Serialization;

namespace EBMS.Infrastructure.DTOs.Auth
{
    public class GetUserDTO
    {
        public string? Message { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public byte[]? ProfilePic { get; set; }
        public List<string>? Roles { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
