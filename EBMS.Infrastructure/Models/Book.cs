namespace EBMS.Infrastructure.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal PhysicalPrice { get; set; }
        public decimal Discount { get; set; }
        public int AvailableQuantity { get; set; }
        public string BookFilePath { get; set; } = null!;
        public string BookCoverImage { get; set; } = null!;
        public DateOnly Published_at { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        // Has One Author
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        // Has Many Reviews
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        // Has Many Wishlists
        public ICollection<Wishlist>? Wishlists { get; set; } = new List<Wishlist>();

        // Has Many BookCategories
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
        // Has Many Categories
        public ICollection<Category> Categories { get; set; } = new List<Category>();

        // Has Many BookOrders
        public ICollection<BookOrder>? BookOrders { get; set; } = new List<BookOrder>();
        // Has Many Orders
        public ICollection<Order>? Orders { get; set; } = new List<Order>();

        // ---> M Users can Download N Books <---
        // Has Many downloads
        public ICollection<BookDownload>? BookDownloads { get; set; } = new List<BookDownload>();
        // Has Many Users
        public ICollection<BookUser>? BookUsers { get; set; } = new List<BookUser>();
    }
}
