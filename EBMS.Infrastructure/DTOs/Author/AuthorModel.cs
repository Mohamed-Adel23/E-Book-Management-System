using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Author
{
    public class AuthorModel
    {
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        public string Bio { get; set; } = null!;
        public IFormFile? ProfilePic { get; set; }
    }
}
