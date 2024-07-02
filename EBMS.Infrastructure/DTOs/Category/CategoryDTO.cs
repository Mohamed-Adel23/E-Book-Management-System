using EBMS.Infrastructure.Models;

namespace EBMS.Infrastructure.DTOs.Category
{
    public class CategoryDTO : BaseDTO
    {
        public string? Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        //public ICollection<Book>? Books { get; set; } = new List<Book>();
    }
}
