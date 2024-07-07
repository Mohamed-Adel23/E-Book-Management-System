namespace EBMS.Infrastructure.Models
{
    public class Wishlist
    {
        public DateTime Created_at { get; set; }
        // Has One User
        public string UserId { get; set; } = null!;
        public BookUser BookUser { get; set; } = null!;
        // Has One Book
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
