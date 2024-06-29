using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Category
{
    public class CategoryModel
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
