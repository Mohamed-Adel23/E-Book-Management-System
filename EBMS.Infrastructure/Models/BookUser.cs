using EBMS.Infrastructure.Models.Auth;
using Microsoft.AspNetCore.Identity;
namespace EBMS.Infrastructure.Models
{
    // This Entity for Users and Authors
    public class BookUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public byte[]? ProfilePic { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        // Has Many Reviews
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        // Has Many Wishlists
        public ICollection<Wishlist>? Wishlists { get; set; } = new List<Wishlist>();
        // Has Many Orders
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
        
        // ---> M Users can Download N Books <---
        // Has Many Book downloads
        public ICollection<BookDownload>? BookDownloads { get; set; } = new List<BookDownload>();
        // Has Many Books
        public ICollection<Book>? Books { get; set; } = new List<Book>();

        // Has Many Refresh Tokens
        public ICollection<BookRefreshToken>? BookRefreshTokens { get; set; } = new List<BookRefreshToken>();
    }
}
