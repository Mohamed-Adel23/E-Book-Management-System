using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Auth
{
    public class UpdatePwdDTO
    {
        [Required]
        public string OldPwd { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string NewPwd { get; set; } = null!;
    }
}