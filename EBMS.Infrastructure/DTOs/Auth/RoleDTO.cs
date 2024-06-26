using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Auth
{
    public class RoleDTO
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Role { get; set; } = null!;
    }
}