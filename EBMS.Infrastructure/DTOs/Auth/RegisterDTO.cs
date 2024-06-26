using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Auth
{
    public class RegisterDTO
    {
        [Required, MaxLength(255)]
        public string FullName { get; set; } = null!;
        [Required, MaxLength(255)]
        public string UserName { get; set; } = null!;
        [Required, MaxLength(255), EmailAddress]
        public string Email { get; set; } = null!;
        [Phone]
        public string? PhoneNumber { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
        public IFormFile? ProfilePic { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required]
        public string Role { get; set; } = null!;
    }
}
