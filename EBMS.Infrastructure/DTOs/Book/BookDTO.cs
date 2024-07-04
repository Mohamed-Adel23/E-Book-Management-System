using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.DTOs.Category;

namespace EBMS.Infrastructure.DTOs.Book
{
    public class BookDTO : BaseDTO
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal PhysicalPrice { get; set; }

        public decimal DownloadPrice { get; set; }

        public decimal Discount { get; set; }

        public int AvailableQuantity { get; set; }

        public decimal Rate { get; set; }

        public string? BookFilePath { get; set; }

        public string? BookCoverImage { get; set; }

        public DateOnly Published_at { get; set; }

        public DateTime Created_at { get; set; }

        public DateTime? Updated_at { get; set; }

        // Author of Book
        public AuthorDTO? Author { get; set; }

        // List of Book Categories
        public List<CategoryDTO>? Categories { get; set; }
    }
}
