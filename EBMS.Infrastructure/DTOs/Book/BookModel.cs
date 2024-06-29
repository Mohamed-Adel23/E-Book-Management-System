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
        
        [Range(0, (double)100000m, ErrorMessage = "Physical price must be a positive value")]
        public decimal PhysicalPrice { get; set; }
        
        [Range(0, (double)100000m, ErrorMessage = "Physical price must be a positive value")]
        public decimal DownloadPrice { get; set; }
        
        public decimal Discount { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Available quantity must be a positive value")]
        public int AvailableQuantity { get; set; }
        
        public IFormFile BookFilePath { get; set; } = null!;
        
        public IFormFile BookCoverImage { get; set; } = null!;
        
        public DateOnly Published_at { get; set; }
        
        public int AuthorId { get; set; }
        
        // This is a string of categories ids seperated by (,)
        public string CategoriesIds { get; set; } = null!;
    }
}
