using Microsoft.AspNetCore.Identity;
namespace EBMS.Infrastructure.Models
{
    // This Entity for Users and Authors
    public class BookUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public byte[]? ProfilePic { get; set; }
        // This Prop belongs to Authors Only, which the author must provide an evidence file and
        // this application will be reviewed by Admins
        public string? AuthorFile { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        // Has Many Reviews
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        // Has Many Wishlists
        public ICollection<Wishlist>? Wishlists { get; set; } = new List<Wishlist>();
        // Has Many Orders
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
        // For Authors Role, Has Many Books
        public ICollection<Book>? Books { get; set; } = new List<Book>();
    }
}
