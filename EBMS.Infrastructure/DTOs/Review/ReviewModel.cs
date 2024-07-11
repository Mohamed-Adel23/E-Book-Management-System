using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Review
{
    public class ReviewModel
    {
        [Required]
        [Range(0, 5, ErrorMessage = "You should rate the book with anumber from 0 to 5")]
        public int Rate { get; set; }
        [Required]
        public string Comment { get; set; } = null!;
        [Required]
        public int BookId { get; set; }
    }
}
