namespace EBMS.Infrastructure.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        // Has Many BookCategories
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
        // Has Many Books
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
