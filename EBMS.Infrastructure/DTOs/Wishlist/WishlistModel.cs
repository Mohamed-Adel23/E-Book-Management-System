using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Wishlist
{
    public class WishlistModel
    {
        [Required]
        public int BookId { get; set; }
    }
}
