using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Auth
{
    public class GetTokenDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
