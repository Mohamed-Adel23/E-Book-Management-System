using EBMS.Infrastructure.Models;

namespace EBMS.Infrastructure.DTOs.Category
{
    public class CategoryDTO
    {
        public string? Message { get; set; }
        public int Id { get; set; }
        public string? Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        //public ICollection<Book>? Books { get; set; } = new List<Book>();
    }
}
