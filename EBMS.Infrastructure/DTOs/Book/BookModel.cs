using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Book
{
    public class BookModel
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        [MaxLength(255)]
        public string? Description { get; set; }
        
        [Range(10, (double)100000m, ErrorMessage = "Physical price must be a positive value in range (10, 100000)")]
        public decimal PhysicalPrice { get; set; }
        
        [Range(10, (double)100000m, ErrorMessage = "Physical price must be a positive value in range (10, 100000)")]
        public decimal DownloadPrice { get; set; }
        
        public decimal Discount { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Available quantity must be a positive value")]
        public int AvailableQuantity { get; set; }
        
        public IFormFile? BookFilePath { get; set; }
        
        public IFormFile? BookCoverImage { get; set; }
        
        public DateOnly Published_at { get; set; }
        
        public int AuthorId { get; set; }
        
        // This is a string of categories ids seperated by (,)
        public string CategoriesIds { get; set; } = null!;
    }
}
